export interface Project {
  id: string;
  name: string;
  street: string | null;
  city: string | null;
  zipCode: string | null;
  gpsLat: number | null;
  gpsLng: number | null;
  clientName: string | null;
  clientContact: string | null;
  status: ProjectStatus;
  createdBy: string;
  createdAt: string;
  updatedAt: string;
  version: number;
}

export type ProjectStatus = "active" | "completed" | "archived";

export interface Zone {
  id: string;
  projectId: string;
  parentZoneId: string | null;
  name: string;
  type: ZoneType;
  qrCode: string | null;
  sortOrder: number | null;
  version: number;
}

export type ZoneType = "building" | "floor" | "room" | "trench";
