const API_LOCAL_HTTPS = "https://localhost:7187";
const API_LOCAL_HTTP = "http://localhost:5091";
const API_PRODUCTION = "https://sistemaparqueadero-api.onrender.com";

export const API_BASE =
    (location.hostname === "localhost" || location.hostname === "127.0.0.1")
        ? (location.protocol === "https:" ? API_LOCAL_HTTPS : API_LOCAL_HTTP)
        : API_PRODUCTION;

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