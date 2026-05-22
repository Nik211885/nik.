import { AngularNodeAppEngine, createNodeRequestHandler, isMainModule, writeResponseToNodeResponse } from '@angular/ssr/node';
import express, { Request, Response, NextFunction } from 'express';
import { dirname, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const serverDistFolder = dirname(fileURLToPath(import.meta.url));
const browserDistFolder = resolve(serverDistFolder, '../browser');

const app = express();
const engine = new AngularNodeAppEngine({
  allowedHosts: (process.env['NG_ALLOWED_HOSTS'] || '').split(',').filter(Boolean),
  trustProxyHeaders: true,
});

const backendUrl = (process.env['BACKEND_URL'] || 'http://localhost:5055').replace(/\/$/, '');

// Proxy API requests to the backend so relative URLs work from Angular SSR
async function proxyToBackend(req: Request, res: Response, next: NextFunction): Promise<void> {
  try {
    const targetUrl = `${backendUrl}${req.originalUrl}`;
    const headers: Record<string, string> = {};
    for (const [key, value] of Object.entries(req.headers)) {
      if (key.toLowerCase() !== 'host' && value) {
        headers[key] = Array.isArray(value) ? value.join(', ') : value;
      }
    }

    let body: Buffer | undefined;
    if (!['GET', 'HEAD'].includes(req.method.toUpperCase())) {
      const chunks: Buffer[] = [];
      await new Promise<void>((resolve, reject) => {
        req.on('data', (chunk: Buffer) => chunks.push(chunk));
        req.on('end', resolve);
        req.on('error', reject);
      });
      if (chunks.length) body = Buffer.concat(chunks);
    }

    const response = await fetch(targetUrl, {
      method: req.method,
      headers,
      body: body as BodyInit | undefined,
    });

    res.status(response.status);
    response.headers.forEach((value, key) => {
      if (key.toLowerCase() !== 'transfer-encoding') res.setHeader(key, value);
    });

    const buf = await response.arrayBuffer();
    res.end(Buffer.from(buf));
  } catch (err) {
    next(err);
  }
}

app.use(['/api', '/public-api'], proxyToBackend);

// Serve static files from /browser
app.use(
  express.static(browserDistFolder, {
    maxAge: '1y',
    index: false,
    redirect: false,
  })
);

// Handle all other requests with Angular SSR
app.use((req, res, next) => {
  engine
    .handle(req)
    .then(response => {
      if (response) {
        writeResponseToNodeResponse(response, res);
      } else {
        next();
      }
    })
    .catch(next);
});

const port = process.env['PORT'] || 4000;

if (isMainModule(import.meta.url)) {
  app.listen(port, () => {
    console.log(`Node Express server listening on http://localhost:${port}`);
  });
}

export const reqHandler = createNodeRequestHandler(app);
