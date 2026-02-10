import React from "react";
import { View, Text, StyleSheet, ScrollView } from "react-native";

interface DiffRow {
  field: string;
  clientValue: string;
  serverValue: string;
  isDifferent: boolean;
}

interface ConflictDiffViewProps {
  clientPayload: string;
  serverPayload: string;
}

function parsePayload(json: string): Record<string, unknown> {
  try {
    return JSON.parse(json) as Record<string, unknown>;
  } catch {
    return {};
  }
}

function formatValue(value: unknown): string {
  if (value === null || value === undefined) return "-";
  if (typeof value === "object") return JSON.stringify(value);
  return String(value);
}

function computeDiff(
  clientPayload: string,
  serverPayload: string
): DiffRow[] {
  const client = parsePayload(clientPayload);
  const server = parsePayload(serverPayload);

  const allKeys = new Set([
    ...Object.keys(client),
    ...Object.keys(server),
  ]);

  return Array.from(allKeys)
    .sort()
    .map((field) => {
      const clientValue = formatValue(client[field]);
      const serverValue = formatValue(server[field]);
      return {
        field,
        clientValue,
        serverValue,
        isDifferent: clientValue !== serverValue,
      };
    });
}

export function ConflictDiffView({
  clientPayload,
  serverPayload,
}: ConflictDiffViewProps) {
  const rows = computeDiff(clientPayload, serverPayload);

  return (
    <ScrollView horizontal>
      <View style={styles.table}>
        <View style={styles.headerRow}>
          <Text style={[styles.cell, styles.fieldCell, styles.headerText]}>
            Feld
          </Text>
          <Text style={[styles.cell, styles.valueCell, styles.headerText]}>
            Ihre Version
          </Text>
          <Text style={[styles.cell, styles.valueCell, styles.headerText]}>
            Server-Version
          </Text>
        </View>
        {rows.map((row) => (
          <View
            key={row.field}
            style={[styles.row, row.isDifferent && styles.diffRow]}
          >
            <Text style={[styles.cell, styles.fieldCell]}>{row.field}</Text>
            <Text style={[styles.cell, styles.valueCell]}>
              {row.clientValue}
            </Text>
            <Text style={[styles.cell, styles.valueCell]}>
              {row.serverValue}
            </Text>
          </View>
        ))}
      </View>
    </ScrollView>
  );
}

const styles = StyleSheet.create({
  table: {
    minWidth: 500,
  },
  headerRow: {
    flexDirection: "row",
    backgroundColor: "#E5E5EA",
    borderBottomWidth: 1,
    borderBottomColor: "#C7C7CC",
  },
  headerText: {
    fontWeight: "700",
    fontSize: 13,
  },
  row: {
    flexDirection: "row",
    borderBottomWidth: StyleSheet.hairlineWidth,
    borderBottomColor: "#E5E5EA",
  },
  diffRow: {
    backgroundColor: "#FFF3CD",
  },
  cell: {
    padding: 8,
    fontSize: 13,
  },
  fieldCell: {
    width: 140,
    fontWeight: "600",
  },
  valueCell: {
    width: 180,
  },
});
