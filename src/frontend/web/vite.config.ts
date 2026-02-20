import { defineConfig } from "vite";
import react from "@vitejs/plugin-react";
import tailwindcss from "@tailwindcss/vite";
import path from "path";

export default defineConfig({
  plugins: [react(), tailwindcss()],
  resolve: {
    alias: {
      "@": path.resolve(__dirname, "./src"),
    },
  },
  server: {
    port: 5173,
    hmr: {
      clientPort: 5000,
    },
  },
  build: {
    rollupOptions: {
      output: {
        manualChunks: {
          router: ["@tanstack/react-router"],
          query: ["@tanstack/react-query"],
          forms: ["react-hook-form", "@hookform/resolvers", "zod"],
          auth: ["oidc-client-ts"],
        },
      },
    },
  },
});
