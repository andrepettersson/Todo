import { api } from "../../helpers/httpClient.js";

export async function login(email, password) {
  return api("accounts/login", {
    method: "POST",
    body: JSON.stringify({ username: email, password })
  });
}

export async function logout() {
  return api("accounts/logout", { method: "POST" });
}