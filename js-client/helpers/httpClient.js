import { API_BASE } from "./config.js";

export async function api(path, options = {}) {
  const init = { credentials: "include", ...options };
  const headers = new Headers(init.headers || {});
  if (init.body && !headers.has("Content-Type")) {
    headers.set("Content-Type", "application/json");
  }
  init.headers = headers;

  const url = API_BASE + path.replace(/^\/+/, "");
  const res = await fetch(url, init);

  if (!res.ok) {
    const text = await res.text().catch(() => "");
    const err = new Error(`HTTP ${res.status}`);
    err.status = res.status;
    err.body = text;
    throw err;
  }

  const ct = res.headers.get("Content-Type") || "";
  if (ct.includes("application/json")) return await res.json();
  return await res.text();
}