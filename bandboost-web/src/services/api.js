const API_URL = import.meta.env.VITE_API_URL || "http://localhost:5229/api";

export async function apiRequest(path, options = {}) {
    const token = localStorage.getItem("token");
    const response = await fetch(`${API_URL}${path}`, {
        ...options,
        headers: {
            ...(options.body ? { "Content-Type": "application/json" } : {}),
            ...(token ? { Authorization: `Bearer ${token}` } : {}),
            ...options.headers
        }
    });

    const contentType = response.headers.get("content-type") || "";
    const data = contentType.includes("application/json") ? await response.json() : null;

    if (!response.ok) {
        const error = new Error(data?.message || "Không thể kết nối đến hệ thống.");
        error.status = response.status;
        throw error;
    }

    return data;
}

export { API_URL };
