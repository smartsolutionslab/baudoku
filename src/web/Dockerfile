FROM node:22-alpine AS build
WORKDIR /app

# Copy workspace root and shared packages
COPY src/package.json src/package-lock.json ./
COPY src/packages/ ./packages/
COPY src/web/package.json ./web/package.json

# Install workspace deps (only web + shared packages, skip frontend)
RUN npm ci --workspace=web --workspace=@baudoku/shared-types --workspace=@baudoku/shared-validation --workspace=@baudoku/shared-api --workspace=@baudoku/shared-constants --workspace=@baudoku/shared-logic

# Copy web source
COPY src/web/ ./web/

# Build
RUN npm run build --workspace=web

FROM nginx:alpine AS final
COPY --from=build /app/web/dist /usr/share/nginx/html
COPY src/web/nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
