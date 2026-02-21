import { openDatabaseSync, type SQLiteDatabase } from "expo-sqlite";
import { drizzle, type ExpoSQLiteDatabase } from "drizzle-orm/expo-sqlite";
import * as schema from "./schema";

let expoDb: SQLiteDatabase | null = null;
let drizzleDb: ExpoSQLiteDatabase<typeof schema> | null = null;

function getExpoDb(): SQLiteDatabase {
  if (!expoDb) {
    expoDb = openDatabaseSync("baudoku.db", { enableChangeListener: true });
    expoDb.execSync("PRAGMA journal_mode = WAL;");
    expoDb.execSync("PRAGMA foreign_keys = ON;");
  }
  return expoDb;
}

function getDrizzleDb(): ExpoSQLiteDatabase<typeof schema> {
  if (!drizzleDb) {
    drizzleDb = drizzle(getExpoDb(), { schema });
  }
  return drizzleDb;
}

// Lazy-initialized proxies — database opens on first access, not at import time.
// This prevents module-level hangs on environments where expo-sqlite
// initialization is slow (e.g. CI emulators with software rendering).
export const db = new Proxy({} as ExpoSQLiteDatabase<typeof schema>, {
  get(_target, prop) {
    return (getDrizzleDb() as any)[prop];
  },
});

export const expoDatabase = new Proxy({} as SQLiteDatabase, {
  get(_target, prop) {
    return (getExpoDb() as any)[prop];
  },
});
