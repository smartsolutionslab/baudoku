import React from "react";
import { View, Text, TouchableOpacity, StyleSheet } from "react-native";
import type { Project } from "../../db/repositories/types";
import { StatusBadge } from "../common/StatusBadge";
import { Colors, Spacing, FontSize } from "../../styles/tokens";

interface ProjectCardProps {
  project: Project;
  onPress: () => void;
}

export function ProjectCard({ project, onPress }: ProjectCardProps) {
  const address = [project.street, project.zipCode, project.city]
    .filter(Boolean)
    .join(", ");

  return (
    <TouchableOpacity style={styles.card} onPress={onPress} activeOpacity={0.7}>
      <View style={styles.header}>
        <Text style={styles.name} numberOfLines={1}>
          {project.name}
        </Text>
        <Text style={styles.chevron}>›</Text>
      </View>
      {address ? (
        <Text style={styles.address} numberOfLines={1}>
          {address}
        </Text>
      ) : null}
      <View style={styles.footer}>
        <StatusBadge status={project.status} />
        {project.clientName ? (
          <Text style={styles.client} numberOfLines={1}>
            {project.clientName}
          </Text>
        ) : null}
      </View>
    </TouchableOpacity>
  );
}

const styles = StyleSheet.create({
  card: {
    backgroundColor: Colors.card,
    borderRadius: 12,
    padding: Spacing.lg,
    marginBottom: Spacing.sm,
  },
  header: {
    flexDirection: "row",
    justifyContent: "space-between",
    alignItems: "center",
    marginBottom: Spacing.xs,
  },
  name: {
    fontSize: FontSize.callout,
    fontWeight: "600",
    color: Colors.textPrimary,
    flex: 1,
  },
  chevron: {
    fontSize: 22,
    color: Colors.textTertiary,
    marginLeft: Spacing.sm,
  },
  address: {
    fontSize: FontSize.caption,
    color: Colors.textTertiary,
    marginBottom: Spacing.sm,
  },
  footer: {
    flexDirection: "row",
    alignItems: "center",
    gap: Spacing.sm,
  },
  client: {
    fontSize: FontSize.caption,
    color: Colors.textSecondary,
    flex: 1,
  },
});
