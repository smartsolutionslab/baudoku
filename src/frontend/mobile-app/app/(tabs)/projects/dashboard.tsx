import {
  View,
  Text,
  ScrollView,
  TouchableOpacity,
  ActivityIndicator,
  StyleSheet,
} from 'react-native';
import { useRouter, Stack } from 'expo-router';
import { FontAwesome } from '@expo/vector-icons';
import { useDashboardStats } from '@/hooks';
import { DashboardCard } from '@/components/common';
import { Colors, Spacing, FontSize, Radius } from '@/styles/tokens';

type QuickLink = {
  key: string;
  icon: React.ComponentProps<typeof FontAwesome>['name'];
  label: string;
  route: string;
};

export default function DashboardScreen() {
  const router = useRouter();
  const { data: stats, isLoading } = useDashboardStats();

  if (isLoading || !stats) {
    return (
      <View style={styles.loading}>
        <Stack.Screen options={{ title: 'Dashboard' }} />
        <ActivityIndicator size="large" color={Colors.primary} />
      </View>
    );
  }

  const QUICK_LINKS: QuickLink[] = [
    { key: 'projects', icon: 'building', label: 'Alle Projekte', route: '/(tabs)/projects/' },
    {
      key: 'search',
      icon: 'search',
      label: 'Installationen suchen',
      route: '/(tabs)/projects/search',
    },
    { key: 'sync', icon: 'refresh', label: 'Synchronisation', route: '/(tabs)/sync' },
  ];

  const {
    projectCount,
    installationCount,
    photoCount,
    unsyncedCount,
    measurementsByResult,
    installationsByStatus,
  } = stats;
  const {
    failed: failedCount = 0,
    passed: passedCount = 0,
    warning: warningCount = 0,
  } = measurementsByResult;
  const { in_progress: inProgressCount = 0, completed: completedCount = 0 } = installationsByStatus;
  const totalMeasurements = failedCount + passedCount + warningCount;

  return (
    <ScrollView style={styles.container} contentContainerStyle={styles.content}>
      <Stack.Screen options={{ title: 'Dashboard' }} />

      <Text style={styles.sectionTitle}>Gesamt</Text>
      <View style={styles.row}>
        <DashboardCard title="Projekte" value={projectCount} />
        <View style={styles.gap} />
        <DashboardCard title="Installationen" value={installationCount} />
      </View>

      <View style={styles.row}>
        <DashboardCard title="Fotos" value={photoCount} />
        <View style={styles.gap} />
        <DashboardCard title="Messungen" value={totalMeasurements} />
      </View>

      <Text style={styles.sectionTitle}>Status</Text>
      <View style={styles.row}>
        <DashboardCard title="In Arbeit" value={inProgressCount} color={Colors.warning} />
        <View style={styles.gap} />
        <DashboardCard title="Fertig" value={completedCount} color={Colors.success} />
      </View>

      <View style={styles.row}>
        <DashboardCard
          title="Bestanden"
          value={passedCount}
          color={Colors.success}
          subtitle="Messungen"
        />
        <View style={styles.gap} />
        <DashboardCard
          title="Fehlgeschlagen"
          value={failedCount}
          color={Colors.danger}
          subtitle="Messungen"
        />
      </View>

      <View style={styles.row}>
        <DashboardCard
          title="Nicht synchronisiert"
          value={unsyncedCount}
          color={unsyncedCount > 0 ? Colors.warning : Colors.success}
        />
        <View style={styles.gap} />
        <View style={{ flex: 1 }} />
      </View>

      <Text style={styles.sectionTitle}>Schnellzugriff</Text>

      {QUICK_LINKS.map(({ key, icon, label, route }) => (
        <TouchableOpacity
          key={key}
          style={styles.quickLink}
          onPress={() => router.push(route as never)}
        >
          <FontAwesome name={icon} size={18} color={Colors.primary} />
          <Text style={styles.quickLinkText}>{label}</Text>
          <FontAwesome name="chevron-right" size={14} color={Colors.textTertiary} />
        </TouchableOpacity>
      ))}
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  content: {
    padding: Spacing.lg,
    paddingBottom: 40,
  },
  loading: {
    flex: 1,
    justifyContent: 'center',
    alignItems: 'center',
    backgroundColor: Colors.background,
  },
  sectionTitle: {
    fontSize: FontSize.headline,
    fontWeight: '600',
    color: Colors.textPrimary,
    marginTop: Spacing.lg,
    marginBottom: Spacing.md,
  },
  row: {
    flexDirection: 'row',
    marginBottom: Spacing.md,
  },
  gap: {
    width: Spacing.md,
  },
  quickLink: {
    flexDirection: 'row',
    alignItems: 'center',
    backgroundColor: Colors.card,
    borderRadius: Radius.md,
    padding: Spacing.lg,
    marginBottom: Spacing.sm,
    gap: Spacing.md,
  },
  quickLinkText: {
    flex: 1,
    fontSize: FontSize.body,
    fontWeight: '500',
    color: Colors.textPrimary,
  },
});
