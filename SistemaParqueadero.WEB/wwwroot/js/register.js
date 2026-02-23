import { apiFetch } from "/js/api.js";

const form = document.getElementById("registerForm");
const btn = document.getElementById("btnRegister");
const msg = document.getElementById("msg");

const nombreCompleto = document.getElementById("nombreCompleto");
const username = document.getElementById("username");
const password = document.getElementById("password");
const password2 = document.getElementById("password2");

form.addEventListener("submit", async (e) => {
    e.preventDefault();

    setMsg("", "");
    const nombre = nombreCompleto.value.trim();
    const user = username.value.trim();
    const pass = password.value;
    const pass2 = password2.value;

    if (!nombre || !user || !pass || !pass2) {
        setMsg("Completa todos los campos.", "err");
        return;
    }

    if (pass.length < 6) {
        setMsg("La contraseña debe tener al menos 6 caracteres.", "err");
        return;
    }

    if (pass !== pass2) {
        setMsg("Las contraseñas no coinciden.", "err");
        return;
    }

    setLoading(true);

    try {
        const res = await apiFetch("/api/auth/register", {
            method: "POST",
            body: JSON.stringify({
                nombreCompleto: nombre,
                username: user,
                password: pass
            })
        });

        const data = await safeJson(res);

        if (!res.ok) {
            setMsg(data?.message || "No se pudo registrar.", "err");
            return;
        }

        setMsg("Registro exitoso. Ahora ingresa.", "ok");

        form.reset();
        setTimeout(() => {
            window.location.href = "/";
        }, 700);

    } catch (err) {
        console.error(err);
        setMsg("No se pudo conectar con el servidor.", "err");
    } finally {
        setLoading(false);
    }
});

function setLoading(state) {
    btn.disabled = state;
    btn.style.opacity = state ? "0.75" : "1";
}

function setMsg(text, type) {
    msg.className = "msg " + (type || "");
    msg.textContent = text || "";
}

async function safeJson(res) {
    try { return await res.json(); } catch { return null; }
}