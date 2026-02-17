import { View, FlatList, Text, StyleSheet } from "react-native";
import { useRouter, Stack } from "expo-router";
import { useInstallationSearch } from "../../../src/hooks";
import { SearchBar, FilterChips, EmptyState } from "../../../src/components/common";
import { InstallationCard } from "../../../src/components/installations";
import { Colors, Spacing, FontSize } from "../../../src/styles/tokens";
import { INSTALLATION_STATUS_OPTIONS } from "../../../src/constants";

export default function SearchScreen() {
  const router = useRouter();
  const { query, setQuery, filters, toggleStatus, results, searching } =
    useInstallationSearch();

  return (
    <View style={styles.container}>
      <Stack.Screen options={{ title: "Installationen suchen" }} />

      <SearchBar
        value={query}
        onChangeText={setQuery}
        placeholder="Typ, Hersteller, Modell..."
      />

      <FilterChips
        options={INSTALLATION_STATUS_OPTIONS}
        selected={filters.status ?? []}
        onToggle={toggleStatus}
      />

      {results.length === 0 && !searching ? (
        <EmptyState
          icon="search"
          title={query || (filters.status?.length ?? 0) > 0 ? "Keine Treffer" : "Suche starten"}
          subtitle={
            query || (filters.status?.length ?? 0) > 0
              ? "Versuche andere Suchbegriffe oder Filter."
              : "Gib einen Suchbegriff ein oder wähle einen Status-Filter."
          }
        />
      ) : (
        <FlatList
          data={results}
          keyExtractor={(item) => item.id}
          renderItem={({ item }) => (
            <View>
              <InstallationCard
                installation={item}
                onPress={() =>
                  router.push(`/(tabs)/projects/installation/${item.id}`)
                }
              />
              <Text style={styles.context}>
                {item.projectName} / {item.zoneName}
              </Text>
            </View>
          )}
          contentContainerStyle={styles.list}
        />
      )}
    </View>
  );
}

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: Colors.background,
  },
  list: {
    paddingHorizontal: Spacing.lg,
    paddingBottom: 40,
  },
  context: {
    fontSize: FontSize.footnote,
    color: Colors.textTertiary,
    marginTop: -Spacing.sm,
    marginBottom: Spacing.md,
    marginLeft: Spacing.md,
  },
});
