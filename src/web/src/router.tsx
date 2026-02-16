import {
  createRouter,
  createRootRouteWithContext,
  createRoute,
  redirect,
} from "@tanstack/react-router";
import { RootLayout } from "./routes/__root";
import { DashboardPage } from "./routes/index";
import { LoginPage } from "./routes/login";
import { ProjectListPage } from "./routes/projects/index";
import { ProjectNewPage } from "./routes/projects/new";
import { ProjectDetailPage } from "./routes/projects/$projectId/index";
import { ZoneNewPage } from "./routes/projects/$projectId/zones/new";
import { InstallationListPage } from "./routes/projects/$projectId/installations/index";
import { InstallationNewPage } from "./routes/projects/$projectId/installations/new";
import { InstallationDetailPage } from "./routes/projects/$projectId/installations/$installationId/index";

// ─── Router Context ─────────────────────────────────────────────

export type RouterContext = {
  auth: {
    isAuthenticated: boolean;
  };
};

// ─── Route Tree ─────────────────────────────────────────────────

const rootRoute = createRootRouteWithContext<RouterContext>()({
  component: RootLayout,
  beforeLoad: ({ context, location }) => {
    if (!context.auth.isAuthenticated && location.pathname !== "/login") {
      throw redirect({ to: "/login" });
    }
  },
});

const indexRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: "/",
  component: DashboardPage,
});

const loginRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: "/login",
  component: LoginPage,
});

const projectsRoute = createRoute({
  getParentRoute: () => rootRoute,
  path: "/projects",
});

const projectListRoute = createRoute({
  getParentRoute: () => projectsRoute,
  path: "/",
  component: ProjectListPage,
});

const projectNewRoute = createRoute({
  getParentRoute: () => projectsRoute,
  path: "/new",
  component: ProjectNewPage,
});

const projectDetailRoute = createRoute({
  getParentRoute: () => projectsRoute,
  path: "/$projectId",
});

const projectDetailIndexRoute = createRoute({
  getParentRoute: () => projectDetailRoute,
  path: "/",
  component: ProjectDetailPage,
});

const zoneNewRoute = createRoute({
  getParentRoute: () => projectDetailRoute,
  path: "/zones/new",
  component: ZoneNewPage,
});

const installationsRoute = createRoute({
  getParentRoute: () => projectDetailRoute,
  path: "/installations",
});

const installationListRoute = createRoute({
  getParentRoute: () => installationsRoute,
  path: "/",
  component: InstallationListPage,
});

const installationNewRoute = createRoute({
  getParentRoute: () => installationsRoute,
  path: "/new",
  component: InstallationNewPage,
});

const installationDetailRoute = createRoute({
  getParentRoute: () => installationsRoute,
  path: "/$installationId",
  component: InstallationDetailPage,
});

// ─── Route Tree Assembly ────────────────────────────────────────

const routeTree = rootRoute.addChildren([
  indexRoute,
  loginRoute,
  projectsRoute.addChildren([
    projectListRoute,
    projectNewRoute,
    projectDetailRoute.addChildren([
      projectDetailIndexRoute,
      zoneNewRoute,
      installationsRoute.addChildren([
        installationListRoute,
        installationNewRoute,
        installationDetailRoute,
      ]),
    ]),
  ]),
]);

export const router = createRouter({
  routeTree,
  context: { auth: undefined! },
});

declare module "@tanstack/react-router" {
  interface Register {
    router: typeof router;
  }
}
