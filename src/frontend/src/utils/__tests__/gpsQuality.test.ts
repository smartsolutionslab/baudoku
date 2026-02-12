import { calculateGpsQuality, gpsSourceLabels } from "../gpsQuality";

describe("calculateGpsQuality", () => {
  // Stage 1: base grade from accuracy only
  it("returns grade A for accuracy < 1m", () => {
    const result = calculateGpsQuality({ gpsAccuracy: 0.5 });
    expect(result.grade).toBe("A");
  });

  it("returns grade B for accuracy 1–5m", () => {
    const result = calculateGpsQuality({ gpsAccuracy: 3.5 });
    expect(result.grade).toBe("B");
  });

  it("returns grade C for accuracy 5–30m", () => {
    const result = calculateGpsQuality({ gpsAccuracy: 15.0 });
    expect(result.grade).toBe("C");
  });

  it("returns grade D for accuracy >= 30m", () => {
    const result = calculateGpsQuality({ gpsAccuracy: 50.0 });
    expect(result.grade).toBe("D");
  });

  // Stage 2–4: secondary factors
  it("upgrades grade by 1 when bonuses outweigh penalties", () => {
    // 4m → base B, HDOP 1.5 (+1), 10 sats (+1), correction (+1) = +3 → clamped +1 → A
    const result = calculateGpsQuality({
      gpsAccuracy: 4.0,
      gpsHdop: 1.5,
      gpsSatCount: 10,
      gpsCorrService: "SAPOS",
    });
    expect(result.grade).toBe("A");
  });

  it("downgrades grade by 1 when penalties outweigh bonuses", () => {
    // 15m → base C, HDOP 6.0 (-1), 3 sats (-1) = -2 → clamped -1 → D
    const result = calculateGpsQuality({
      gpsAccuracy: 15.0,
      gpsHdop: 6.0,
      gpsSatCount: 3,
    });
    expect(result.grade).toBe("D");
  });

  it("caps negative adjustment at -1 (A cannot drop below B)", () => {
    // 0.5m → base A, HDOP 6.0 (-1), 3 sats (-1) = -2 → clamped -1 → B
    const result = calculateGpsQuality({
      gpsAccuracy: 0.5,
      gpsHdop: 6.0,
      gpsSatCount: 3,
    });
    expect(result.grade).toBe("B");
  });

  it("caps positive adjustment at +1 (D cannot rise above C)", () => {
    // 50m → base D, HDOP 1.0 (+1), 12 sats (+1), correction (+1) = +3 → clamped +1 → C
    const result = calculateGpsQuality({
      gpsAccuracy: 50.0,
      gpsHdop: 1.0,
      gpsSatCount: 12,
      gpsCorrService: "SAPOS",
    });
    expect(result.grade).toBe("C");
  });

  it("returns base grade when no secondary factors provided", () => {
    const result = calculateGpsQuality({ gpsAccuracy: 3.5 });
    expect(result.grade).toBe("B");
  });

  it("cannot improve beyond grade A", () => {
    // 0.5m → base A, all bonuses → clamped +1 but already at 0 → stays A
    const result = calculateGpsQuality({
      gpsAccuracy: 0.5,
      gpsHdop: 1.0,
      gpsSatCount: 12,
      gpsCorrService: "SAPOS",
    });
    expect(result.grade).toBe("A");
  });

  it("cannot worsen beyond grade D", () => {
    // 50m → base D, all penalties → clamped -1 but already at 3 → stays D
    const result = calculateGpsQuality({
      gpsAccuracy: 50.0,
      gpsHdop: 6.0,
      gpsSatCount: 3,
    });
    expect(result.grade).toBe("D");
  });

  // Label and color output
  it("returns correct label for each grade", () => {
    expect(calculateGpsQuality({ gpsAccuracy: 0.5 }).label).toBe("Ausgezeichnet");
    expect(calculateGpsQuality({ gpsAccuracy: 3.5 }).label).toBe("Gut");
    expect(calculateGpsQuality({ gpsAccuracy: 15.0 }).label).toBe("Akzeptabel");
    expect(calculateGpsQuality({ gpsAccuracy: 50.0 }).label).toBe("Ungenau");
  });

  it("returns correct colors for each grade", () => {
    const a = calculateGpsQuality({ gpsAccuracy: 0.5 });
    expect(a.color).toBe("#34C759");
    expect(a.bgColor).toBe("#E8F5E9");

    const b = calculateGpsQuality({ gpsAccuracy: 3.5 });
    expect(b.color).toBe("#A8D500");
    expect(b.bgColor).toBe("#F1F8E9");

    const c = calculateGpsQuality({ gpsAccuracy: 15.0 });
    expect(c.color).toBe("#FF9500");
    expect(c.bgColor).toBe("#FFF8E1");

    const d = calculateGpsQuality({ gpsAccuracy: 50.0 });
    expect(d.color).toBe("#FF3B30");
    expect(d.bgColor).toBe("#FFEBEE");
  });
});

describe("gpsSourceLabels", () => {
  it("maps internal_gps to German label", () => {
    expect(gpsSourceLabels["internal_gps"]).toBe("Internes GPS");
  });

  it("maps external_dgnss to German label", () => {
    expect(gpsSourceLabels["external_dgnss"]).toBe("Externes DGNSS");
  });

  it("maps external_rtk to German label", () => {
    expect(gpsSourceLabels["external_rtk"]).toBe("Externes RTK");
  });
});
