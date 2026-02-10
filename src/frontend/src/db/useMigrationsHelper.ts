import { useEffect, useState } from "react";
import { useMigrations } from "drizzle-orm/expo-sqlite/migrator";
import { db } from "./client";
import migrations from "../../drizzle/migrations";

export function useMigrationsHelper() {
  const { success, error } = useMigrations(db, migrations);

  return {
    migrationSuccess: success,
    migrationError: error,
  };
}
