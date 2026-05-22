# Pre-built locally with: npm run build -- --configuration production
FROM nginx:1.27-alpine AS runtime

COPY dist/font-end/browser /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
