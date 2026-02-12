import React from "react";
import { View, Text, StyleSheet } from "react-native";
import type { Installation } from "../../db/repositories/types";
import { QualityIndicator } from "./QualityIndicator";
import { Colors, Spacing, FontSize } from "../../styles/tokens";

interface InstallationDetailsProps {
  installation: Installation;
}

function Row({ label, value }: { label: string; value?: string | number | null }) {
  if (value == null || value === "") return null;
  return (
    <View style={styles.row}>
      <Text style={styles.label}>{label}</Text>
      <Text style={styles.value}>{String(value)}</Text>
    </View>
  );
}

const gpsSourceLabels: Record<string, string> = {
  internal_gps: "Internes GPS",
  external_dgnss: "Externes DGNSS",
  external_rtk: "Externes RTK",
};

const corrServiceLabels: Record<string, string> = {
  none: "Keine",
  sapos_eps: "SAPOS-EPS",
  sapos_heps: "SAPOS-HEPS",
  sapos_gpps: "SAPOS-GPPS",
};

const rtkLabels: Record<string, string> = {
  no_fix: "Kein Fix",
  autonomous: "Autonom",
  dgps: "DGPS",
  rtk_float: "RTK Float",
  rtk_fixed: "RTK Fixed",
};

export function InstallationDetails({ installation }: InstallationDetailsProps) {
  const i = installation;
  const hasGps = i.gpsLat != null && i.gpsLng != null;
  const hasCable = i.cableType || i.crossSectionMm2 || i.lengthM;
  const hasElectrical =
    i.circuitId || i.fuseType || i.fuseRatingA || i.voltageV || i.phase;
  const hasTech = hasGps || hasCable || hasElectrical || i.depthMm;

  if (!hasTech) return null;

  return (
    <View style={styles.card}>
      <Text style={styles.cardTitle}>Technische Details</Text>

      {hasGps && (
        <>
          <Text style={styles.subTitle}>GPS / GNSS</Text>
          {i.gpsAccuracy != null && (
            <View style={styles.qualityRow}>
              <QualityIndicator
                gpsAccuracy={i.gpsAccuracy}
                gpsHdop={i.gpsHdop}
                gpsSatCount={i.gpsSatCount}
                gpsCorrService={i.gpsCorrService}
                gpsSource={i.gpsSource}
              />
            </View>
          )}
          <Row label="Breitengrad" value={i.gpsLat?.toFixed(6)} />
          <Row label="Längengrad" value={i.gpsLng?.toFixed(6)} />
          <Row
            label="Genauigkeit"
            value={i.gpsAccuracy != null ? `${i.gpsAccuracy.toFixed(2)} m` : null}
          />
          <Row
            label="Quelle"
            value={i.gpsSource ? gpsSourceLabels[i.gpsSource] ?? i.gpsSource : null}
          />
          <Row
            label="Korrekturdienst"
            value={
              i.gpsCorrService
                ? corrServiceLabels[i.gpsCorrService] ?? i.gpsCorrService
                : null
            }
          />
          <Row
            label="RTK-Status"
            value={
              i.gpsRtkStatus
                ? rtkLabels[i.gpsRtkStatus] ?? i.gpsRtkStatus
                : null
            }
          />
          <Row
            label="Satelliten"
            value={i.gpsSatCount != null ? String(i.gpsSatCount) : null}
          />
          <Row
            label="HDOP"
            value={i.gpsHdop != null ? i.gpsHdop.toFixed(1) : null}
          />
          <Row
            label="Korrekturalter"
            value={i.gpsCorrAge != null ? `${i.gpsCorrAge.toFixed(1)} s` : null}
          />
        </>
      )}

      {hasCable && (
        <>
          <Text style={styles.subTitle}>Kabel</Text>
          <Row label="Typ" value={i.cableType} />
          <Row
            label="Querschnitt"
            value={i.crossSectionMm2 != null ? `${i.crossSectionMm2} mm²` : null}
          />
          <Row
            label="Länge"
            value={i.lengthM != null ? `${i.lengthM} m` : null}
          />
        </>
      )}

      {hasElectrical && (
        <>
          <Text style={styles.subTitle}>Elektrik</Text>
          <Row label="Stromkreis" value={i.circuitId} />
          <Row label="Sicherung" value={i.fuseType} />
          <Row
            label="Nennstrom"
            value={i.fuseRatingA != null ? `${i.fuseRatingA} A` : null}
          />
          <Row
            label="Spannung"
            value={i.voltageV != null ? `${i.voltageV} V` : null}
          />
          <Row label="Phase" value={i.phase} />
        </>
      )}

      {i.depthMm != null && (
        <>
          <Text style={styles.subTitle}>Verlegetiefe</Text>
          <Row label="Tiefe" value={`${i.depthMm} mm`} />
        </>
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  card: {
    backgroundColor: Colors.card,
    marginHorizontal: Spacing.lg,
    marginBottom: Spacing.lg,
    borderRadius: 12,
    padding: Spacing.lg,
  },
  cardTitle: {
    fontSize: FontSize.headline,
    fontWeight: "600",
    marginBottom: Spacing.md,
  },
  subTitle: {
    fontSize: FontSize.body,
    fontWeight: "600",
    color: Colors.primary,
    marginTop: Spacing.md,
    marginBottom: Spacing.xs,
  },
  qualityRow: {
    marginBottom: Spacing.sm,
  },
  row: {
    flexDirection: "row",
    justifyContent: "space-between",
    paddingVertical: Spacing.xs,
  },
  label: {
    fontSize: FontSize.body,
    color: Colors.textTertiary,
  },
  value: {
    fontSize: FontSize.body,
    fontWeight: "500",
    color: Colors.textPrimary,
    flex: 1,
    textAlign: "right",
    marginLeft: Spacing.lg,
  },
});
