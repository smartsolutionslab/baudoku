import { useState, useCallback } from "react";
import { View } from "react-native";
import { ZoneCard } from "./ZoneCard";
import type { ZoneNode } from "../../hooks";
import type { ZoneId } from "../../types/branded";

type ZoneTreeProps = {
  nodes: ZoneNode[];
  onZonePress: (zoneId: ZoneId) => void;
};

export function ZoneTree({ nodes, onZonePress }: ZoneTreeProps) {
  const [collapsed, setCollapsed] = useState<Set<string>>(new Set());

  const toggle = useCallback((id: string) => {
    setCollapsed((prev) => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
  }, []);

  function renderNodes(list: ZoneNode[]) {
    return list.map((node) => {
      const isExpanded = !collapsed.has(node.zone.id);
      return (
        <View key={node.zone.id}>
          <ZoneCard
            node={node}
            expanded={isExpanded}
            onPress={() => onZonePress(node.zone.id)}
            onToggle={() => toggle(node.zone.id)}
          />
          {isExpanded && node.children.length > 0 && renderNodes(node.children)}
        </View>
      );
    });
  }

  return <View>{renderNodes(nodes)}</View>;
}
