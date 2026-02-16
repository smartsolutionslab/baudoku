import { View, Text, TouchableOpacity, StyleSheet } from "react-native";
import type { ZoneNode } from "../../hooks/useZoneTree";
import { StatusBadge } from "../common/StatusBadge";
import { Colors, Spacing, FontSize, Radius } from "../../styles/tokens";

type ZoneCardProps = {
  node: ZoneNode;
  expanded?: boolean;
  onPress: () => void;
  onToggle?: () => void;
};

export function ZoneCard({ node, expanded, onPress, onToggle }: ZoneCardProps) {
  const hasChildren = node.children.length > 0;

  return (
    <TouchableOpacity
      style={[styles.card, { marginLeft: node.level * 24 }]}
      onPress={onPress}
      activeOpacity={0.7}
    >
      <View style={styles.row}>
        {hasChildren ? (
          <TouchableOpacity onPress={onToggle} style={styles.toggle}>
            <Text style={styles.chevron}>{expanded ? "∨" : "›"}</Text>
          </TouchableOpacity>
        ) : (
          <View style={styles.togglePlaceholder} />
        )}
        <View style={styles.info}>
          <Text style={styles.name} numberOfLines={1}>
            {node.zone.name}
          </Text>
          <View style={styles.meta}>
            <StatusBadge status={node.zone.type} />
            {hasChildren && (
              <Text style={styles.count}>
                {node.children.length}{" "}
                {node.children.length === 1 ? "Kind" : "Kinder"}
              </Text>
            )}
          </View>
        </View>
        <Text style={styles.navChevron}>›</Text>
      </View>
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  card: {
    backgroundColor: Colors.card,
    borderRadius: Radius.md,
    padding: Spacing.md,
    marginBottom: Spacing.sm,
  },
  row: {
    flexDirection: "row",
    alignItems: "center",
  },
  toggle: {
    width: 28,
    height: 28,
    justifyContent: "center",
    alignItems: "center",
  },
  togglePlaceholder: {
    width: 28,
  },
  chevron: {
    fontSize: 18,
    color: Colors.textTertiary,
    fontWeight: "600",
  },
  info: {
    flex: 1,
    marginLeft: Spacing.sm,
  },
  name: {
    fontSize: FontSize.body,
    fontWeight: "600",
    color: Colors.textPrimary,
    marginBottom: 2,
  },
  meta: {
    flexDirection: "row",
    alignItems: "center",
    gap: Spacing.sm,
  },
  count: {
    fontSize: FontSize.footnote,
    color: Colors.textTertiary,
  },
  navChevron: {
    fontSize: 22,
    color: Colors.textTertiary,
  },
});
