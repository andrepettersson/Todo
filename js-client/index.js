const baseApiUrl = 'https://localhost:5001/api';

const loginBtn = document.getElementById('loginBtn');
const logoutBtn = document.getElementById('logoutBtn');
const addTodoBtn = document.getElementById('addTodoBtn');
const todoList = document.getElementById('list');

loginBtn.addEventListener('click', login);
logoutBtn.addEventListener('click', logout);
addTodoBtn.addEventListener('click', addTodo);

async function login() {
    const email = document.getElementById('user').value;
    const password = document.getElementById('pass').value;

    await fetch(`${baseApiUrl}/accounts/login`, {
        method: 'POST',
        credentials: 'include',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username: email, password })
    });

    loadTodos();
}

async function logout() {
    await fetch(`${baseApiUrl}/accounts/logout`, {
        method: 'POST',
        credentials: 'include'
    });
    todoList.innerHTML = '';
}

async function addTodo() {
    const title = document.getElementById('todo').value.trim();
    if (!title) return;

    await fetch(`${baseApiUrl}/todo`, {
        method: 'POST',
        credentials: 'include',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ title })
    });

    document.getElementById('todo').value = '';
    loadTodos();
}

async function loadTodos() {
    const response = await fetch(`${baseApiUrl}/todo`, {
        method: 'GET',
        credentials: 'include'
    });

    if (response.ok) {
        const todos = await response.json();
        todoList.innerHTML = '';
        (todos || []).forEach(todo => {
            const li = document.createElement('li');
            li.textContent = todo.title + (todo.isDone ? ' ✅' : '');
            const delBtn = document.createElement('button');
            delBtn.textContent = 'Ta bort';
            delBtn.onclick = () => deleteTodo(todo.id);
            li.appendChild(delBtn);
            todoList.appendChild(li);
        });
    } else {
        todoList.innerHTML = '<li>Inte inloggad</li>';
    }
}

async function deleteTodo(id) {
    await fetch(`${baseApiUrl}/todo/${id}`, {
        method: 'DELETE',
        credentials: 'include'
    });
    loadTodos();
}

loadTodos();