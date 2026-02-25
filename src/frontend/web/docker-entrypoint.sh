#!/bin/sh
cat > /usr/share/nginx/html/config.js <<EOF
window.__BAUDOKU_CONFIG__ = {
  KEYCLOAK_URL: "${KEYCLOAK_URL:-http://localhost:8080}",
  KEYCLOAK_REALM: "${KEYCLOAK_REALM:-baudoku}",
  KEYCLOAK_CLIENT_ID: "${KEYCLOAK_CLIENT_ID:-baudoku-web}"
};
EOF
exec nginx -g 'daemon off;'
