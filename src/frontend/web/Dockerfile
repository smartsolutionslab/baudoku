FROM node:22-alpine AS build
WORKDIR /app

# Copy workspace root and shared packages
COPY src/frontend/package.json src/frontend/package-lock.json ./
COPY src/frontend/packages/ ./packages/
COPY src/frontend/web/package.json ./web/package.json

# Install workspace deps (only web + shared packages, skip mobile-app)
RUN npm ci --workspace=web --workspace=@baudoku/shared-types --workspace=@baudoku/shared-validation --workspace=@baudoku/shared-api --workspace=@baudoku/shared-constants --workspace=@baudoku/shared-logic

# Copy web source
COPY src/frontend/web/ ./web/

# Build
RUN npm run build --workspace=web

FROM nginx:alpine AS final
COPY --from=build /app/web/dist /usr/share/nginx/html
COPY src/frontend/web/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
