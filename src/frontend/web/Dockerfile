FROM node:22-alpine AS build
WORKDIR /app

# Copy workspace root and shared packages
COPY src/frontend/package.json src/frontend/package-lock.json ./
COPY src/frontend/packages/ ./packages/
COPY src/frontend/web/package.json ./web/package.json

# Install workspace deps (only web + shared packages, skip mobile-app)
RUN npm ci --workspace=web --workspace=@baudoku/core --workspace=@baudoku/projects --workspace=@baudoku/documentation

# Copy web source
COPY src/frontend/web/ ./web/

# Build
RUN npm run build --workspace=web

FROM nginx:stable-alpine-slim AS final
# Remove default config
RUN rm -rf /usr/share/nginx/html/*
COPY --from=build /app/web/dist /usr/share/nginx/html
COPY src/frontend/web/nginx.conf /etc/nginx/conf.d/default.conf
COPY src/frontend/web/docker-entrypoint.sh /docker-entrypoint.sh
RUN chmod +x /docker-entrypoint.sh
EXPOSE 80
ENTRYPOINT ["/docker-entrypoint.sh"]
