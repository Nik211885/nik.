# Pre-built locally with: npm run build -- --configuration production
# Output: dist/font-end/browser/ contains browser/ and server/ subdirectories
FROM node:22-alpine AS runtime

WORKDIR /app

# Copy server bundle and browser assets
COPY dist/font-end/browser ./dist

EXPOSE 4000

ENV PORT=4000
ENV NODE_ENV=production

CMD ["node", "dist/server/server.mjs"]
