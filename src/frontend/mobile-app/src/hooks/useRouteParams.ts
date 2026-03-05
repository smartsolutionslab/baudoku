import { useLocalSearchParams } from 'expo-router';
import { requiredParam } from '../utils';
import {
  projectId as toProjectId,
  zoneId as toZoneId,
  installationId as toInstallationId,
} from '@baudoku/core';
import type { ProjectId, ZoneId, InstallationId } from '@baudoku/core';

export function useProjectIdParam(key = 'id'): ProjectId {
  const params = useLocalSearchParams<Record<string, string>>();
  return toProjectId(requiredParam(params[key]));
}

export function useZoneIdParam(key = 'zoneId'): ZoneId {
  const params = useLocalSearchParams<Record<string, string>>();
  return toZoneId(requiredParam(params[key]));
}

export function useInstallationIdParam(key = 'id'): InstallationId {
  const params = useLocalSearchParams<Record<string, string>>();
  return toInstallationId(requiredParam(params[key]));
}
