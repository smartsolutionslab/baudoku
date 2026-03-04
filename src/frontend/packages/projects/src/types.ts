import type { ProjectId, ZoneId, Latitude, Longitude } from '@baudoku/core';

export type Project = {
  id: ProjectId;
  name: string;
  street: string | null;
  city: string | null;
  zipCode: string | null;
  gpsLat: Latitude | null;
  gpsLng: Longitude | null;
  clientName: string | null;
  clientContact: string | null;
  status: ProjectStatus;
  createdBy: string;
  createdAt: string;
  updatedAt: string;
  version: number;
};

export type ProjectStatus = 'active' | 'completed' | 'archived';

export type Zone = {
  id: ZoneId;
  projectId: ProjectId;
  parentZoneId: ZoneId | null;
  name: string;
  type: ZoneType;
  qrCode: string | null;
  sortOrder: number | null;
  version: number;
};

export type ZoneType = 'building' | 'floor' | 'room' | 'trench';
