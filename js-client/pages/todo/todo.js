import { api } from "../../helpers/httpClient.js";

export async function getTodos() {
  return api("todo");
}

export async function createTodo(title) {
  return api("todo", {
    method: "POST",
    body: JSON.stringify({ title })
  });
}

export async function updateTodo(id, { title, isDone }) {
  return api(`todo/${id}`, {
    method: "PUT",
    body: JSON.stringify({ title, isDone })
  });
}

export async function deleteTodo(id) {
  return api(`todo/${id}`, { method: "DELETE" });
}