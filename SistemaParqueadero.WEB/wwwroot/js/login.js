import { apiFetch } from "/js/api.js";
import { parseJwt } from "/js/auth.js";

const form = document.getElementById("loginForm");
const btn = document.getElementById("btnLogin");
const msg = document.getElementById("msg");
const remember = document.getElementById("remember");
const usernameInput = document.getElementById("username");
const passwordInput = document.getElementById("password");

form.addEventListener("submit", async (e) => {
    e.preventDefault();

    msg.className = "msg";
    msg.textContent = "";

    const username = usernameInput.value.trim();
    const password = passwordInput.value;

    if (!username || !password) {
        showError("Completa todos los campos.");
        return;
    }

    setLoading(true);

    try {
        const response = await apiFetch("/api/auth/login", {
            method: "POST",
            body: JSON.stringify({ username, password })
        });

        const data = await response.json();

        if (!response.ok) {
            showError(data?.message || "Credenciales inválidas.");
            return;
        }

        // Guardar token
        if (remember.checked) {
            localStorage.setItem("jwt", data.token);
        } else {
            sessionStorage.setItem("jwt", data.token);
        }

        showSuccess("Ingreso correcto...");

        // Decodificar rol del token
        const payload = parseJwt(data.token);
        const role = payload?.role ||
            payload?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"];

        setTimeout(() => {
            if (role === "Administrador") {
                window.location.href = "/admin.html";
            } else if (role === "Operador") {
                window.location.href = "/operador.html";
            } else {
                window.location.href = "/usuario.html";
            }
        }, 500);

    } catch (error) {
        showError("No se pudo conectar con el servidor.");
        console.error(error);
    } finally {
        setLoading(false);
    }
});

function setLoading(state) {
    if (state) {
        btn.disabled = true;
        btn.style.opacity = "0.7";
    } else {
        btn.disabled = false;
        btn.style.opacity = "1";
    }
}

function showError(text) {
    msg.classList.remove("ok");
    msg.classList.add("err");
    msg.textContent = text;
}

function showSuccess(text) {
    msg.classList.remove("err");
    msg.classList.add("ok");
    msg.textContent = text;
}