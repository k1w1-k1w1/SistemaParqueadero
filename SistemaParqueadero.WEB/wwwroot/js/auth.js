export function saveToken(token) {
    localStorage.setItem("jwt", token);
}

export function getToken() {
    return localStorage.getItem("jwt") || sessionStorage.getItem("jwt");
}

export function logout() {
    localStorage.removeItem("jwt");
    sessionStorage.removeItem("jwt");
    window.location.href = "/";
}

export function parseJwt(token) {
    try {
        const base64Url = token.split(".")[1];
        const base64 = base64Url.replace(/-/g, "+").replace(/_/g, "/");
        const jsonPayload = decodeURIComponent(
            atob(base64)
                .split("")
                .map(c => "%" + ("00" + c.charCodeAt(0).toString(16)).slice(-2))
                .join("")
        );
        return JSON.parse(jsonPayload);
    } catch {
        return null;
    }
}

export function getRole() {
    const token = getToken();
    if (!token) return null;

    const payload = parseJwt(token);
    return payload?.role ||
        payload?.["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"] ||
        null;
}