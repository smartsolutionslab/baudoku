const marks: Record<string, number> = {};
const startTime = performance.now();

export function markStartup(phase: string): void {
  marks[phase] = performance.now();
}

export function logStartupSummary(): void {
  const entries = Object.entries(marks).sort(([, a], [, b]) => a - b);
  console.log('=== STARTUP TIMING ===');
  console.log('JS Bundle Ready: 0ms (baseline)');
  let prev = startTime;
  for (const [phase, time] of entries) {
    const fromStart = Math.round(time - startTime);
    const delta = Math.round(time - prev);
    console.log(`${phase}: ${fromStart}ms (+${delta}ms)`);
    prev = time;
  }
  const total = Math.round(performance.now() - startTime);
  console.log(`Total (to summary): ${total}ms`);
  console.log('======================');
}
