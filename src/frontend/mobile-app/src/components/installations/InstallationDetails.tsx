import { View, Text, StyleSheet } from 'react-native';
import type { Installation } from '../../db/repositories/types';
import { QualityIndicator } from './QualityIndicator';
import { DetailRow } from '../common/DetailRow';
import { gpsSourceLabels, corrServiceLabels, rtkLabels } from '../../utils';
import { Colors, Spacing, FontSize, Radius } from '../../styles/tokens';

type InstallationDetailsProps = {
  installation: Installation;
};

export function InstallationDetails({ installation }: InstallationDetailsProps) {
  const {
    gpsLat,
    gpsLng,
    gpsAccuracy,
    gpsHdop,
    gpsSatCount,
    gpsCorrService,
    gpsSource,
    gpsRtkStatus,
    gpsCorrAge,
    cableType,
    crossSectionMm2,
    lengthM,
    circuitId,
    fuseType,
    fuseRatingA,
    voltageV,
    phase,
    depthMm,
  } = installation;

  const hasGps = gpsLat != null && gpsLng != null;
  const hasCable = cableType || crossSectionMm2 || lengthM;
  const hasElectrical = circuitId || fuseType || fuseRatingA || voltageV || phase;
  const hasTech = hasGps || hasCable || hasElectrical || depthMm;

  if (!hasTech) return null;

  return (
    <View style={styles.card}>
      <Text style={styles.cardTitle}>Technische Details</Text>

      {hasGps && (
        <>
          <Text style={styles.subTitle}>GPS / GNSS</Text>
          {gpsAccuracy != null && (
            <View style={styles.qualityRow}>
              <QualityIndicator
                horizontalAccuracy={gpsAccuracy}
                hdop={gpsHdop}
                satelliteCount={gpsSatCount}
                correctionService={gpsCorrService}
                gpsSource={gpsSource}
              />
            </View>
          )}
          <DetailRow label="Breitengrad" value={gpsLat?.toFixed(6)} />
          <DetailRow label="Längengrad" value={gpsLng?.toFixed(6)} />
          <DetailRow
            label="Genauigkeit"
            value={gpsAccuracy != null ? `${gpsAccuracy.toFixed(2)} m` : null}
          />
          <DetailRow
            label="Quelle"
            value={gpsSource ? (gpsSourceLabels[gpsSource] ?? gpsSource) : null}
          />
          <DetailRow
            label="Korrekturdienst"
            value={gpsCorrService ? (corrServiceLabels[gpsCorrService] ?? gpsCorrService) : null}
          />
          <DetailRow
            label="RTK-Status"
            value={gpsRtkStatus ? (rtkLabels[gpsRtkStatus] ?? gpsRtkStatus) : null}
          />
          <DetailRow label="Satelliten" value={gpsSatCount != null ? String(gpsSatCount) : null} />
          <DetailRow label="HDOP" value={gpsHdop != null ? gpsHdop.toFixed(1) : null} />
          <DetailRow
            label="Korrekturalter"
            value={gpsCorrAge != null ? `${gpsCorrAge.toFixed(1)} s` : null}
          />
        </>
      )}

      {hasCable && (
        <>
          <Text style={styles.subTitle}>Kabel</Text>
          <DetailRow label="Typ" value={cableType} />
          <DetailRow
            label="Querschnitt"
            value={crossSectionMm2 != null ? `${crossSectionMm2} mm²` : null}
          />
          <DetailRow label="Länge" value={lengthM != null ? `${lengthM} m` : null} />
        </>
      )}

      {hasElectrical && (
        <>
          <Text style={styles.subTitle}>Elektrik</Text>
          <DetailRow label="Stromkreis" value={circuitId} />
          <DetailRow label="Sicherung" value={fuseType} />
          <DetailRow label="Nennstrom" value={fuseRatingA != null ? `${fuseRatingA} A` : null} />
          <DetailRow label="Spannung" value={voltageV != null ? `${voltageV} V` : null} />
          <DetailRow label="Phase" value={phase} />
        </>
      )}

      {depthMm != null && (
        <>
          <Text style={styles.subTitle}>Verlegetiefe</Text>
          <DetailRow label="Tiefe" value={`${depthMm} mm`} />
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
    borderRadius: Radius.lg,
    padding: Spacing.lg,
  },
  cardTitle: {
    fontSize: FontSize.headline,
    fontWeight: '600',
    marginBottom: Spacing.md,
  },
  subTitle: {
    fontSize: FontSize.body,
    fontWeight: '600',
    color: Colors.primary,
    marginTop: Spacing.md,
    marginBottom: Spacing.xs,
  },
  qualityRow: {
    marginBottom: Spacing.sm,
  },
});
