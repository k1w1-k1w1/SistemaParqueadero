export const API_BASE = "https://sistemaparqueadero-api.onrender.com";

export async function apiFetch(path, options = {}) {
    const token = localStorage.getItem("jwt") || sessionStorage.getItem("jwt");

    const headers = {
        "Content-Type": "application/json",
        ...(options.headers || {})
    };

    if (token) headers["Authorization"] = `Bearer ${token}`;

    const res = await fetch(`${API_BASE}${path}`, { ...options, headers });

    if (res.status === 401 || res.status === 403) {
        localStorage.removeItem("jwt");
        sessionStorage.removeItem("jwt");
        window.location.href = "/";
        return res;
    }

    return res;
}