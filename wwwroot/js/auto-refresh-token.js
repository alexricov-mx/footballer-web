/**
 * Auto Refresh Token Service
 * Middleware transparente para manejar refresh tokens automáticamente
 */
class AutoRefreshTokenService {
    constructor(options = {}) {
        this.apiBaseUrl = options.apiBaseUrl || 'https://localhost:8090';
        this.onTokenRefresh = options.onTokenRefresh || (() => {});
        this.onAuthRequired = options.onAuthRequired || (() => {});
        this.refreshBuffer = options.refreshBuffer || 5 * 60 * 1000; // 5 minutes in ms
        this.refreshTimer = null;
        
        this.init();
    }

    init() {
        this.setupHttpInterceptor();
        this.scheduleTokenRefresh();
    }

    /**
     * Intercepta todas las requests para agregar tokens y manejar refresh automático
     */
    setupHttpInterceptor() {
        const originalFetch = window.fetch;
        
        window.fetch = async (url, options = {}) => {
            // Agregar access token a todas las requests
            const accessToken = this.getAccessToken();
            if (accessToken && !this.isAuthEndpoint(url)) {
                options.headers = {
                    ...options.headers,
                    'Authorization': `Bearer ${accessToken}`,
                    'X-Refresh-Token': this.getRefreshToken() || ''
                };
            }

            try {
                const response = await originalFetch(url, options);
                
                // Verificar si hay nuevos tokens en la response
                this.handleTokenHeaders(response);
                
                // Si es 401 y no es un endpoint de auth, intentar refresh
                if (response.status === 401 && !this.isAuthEndpoint(url)) {
                    const refreshed = await this.attemptTokenRefresh();
                    
                    if (refreshed) {
                        // Reintentar la request original con el nuevo token
                        options.headers['Authorization'] = `Bearer ${this.getAccessToken()}`;
                        return await originalFetch(url, options);
                    } else {
                        // No se pudo refrescar, notificar que se necesita auth
                        this.onAuthRequired();
                        return response;
                    }
                }
                
                return response;
            } catch (error) {
                console.error('Network error:', error);
                throw error;
            }
        };
    }

    /**
     * Maneja los headers de respuesta para tokens renovados
     */
    handleTokenHeaders(response) {
        const newAccessToken = response.headers.get('X-New-Access-Token');
        const newRefreshToken = response.headers.get('X-New-Refresh-Token');
        
        if (newAccessToken) {
            this.setAccessToken(newAccessToken);
            console.log('✅ Access token refreshed automatically');
        }
        
        if (newRefreshToken) {
            this.setRefreshToken(newRefreshToken);
        }
        
        if (newAccessToken || newRefreshToken) {
            this.onTokenRefresh({
                accessToken: newAccessToken,
                refreshToken: newRefreshToken
            });
            
            // Reprogramar el próximo refresh
            this.scheduleTokenRefresh();
        }
    }

    /**
     * Intenta refrescar el token manualmente
     */
    async attemptTokenRefresh() {
        const refreshToken = this.getRefreshToken();
        if (!refreshToken) {
            console.warn('No refresh token available');
            return false;
        }

        try {
            const response = await fetch(`${this.apiBaseUrl}/api/token/refresh`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({ refreshToken })
            });

            if (response.ok) {
                const tokens = await response.json();
                this.setAccessToken(tokens.accessToken);
                this.setRefreshToken(tokens.refreshToken);
                
                this.onTokenRefresh(tokens);
                this.scheduleTokenRefresh();
                
                console.log('✅ Tokens refreshed manually');
                return true;
            } else {
                console.warn('Failed to refresh token:', response.status);
                return false;
            }
        } catch (error) {
            console.error('Error refreshing token:', error);
            return false;
        }
    }

    /**
     * Programa el próximo refresh automático
     */
    scheduleTokenRefresh() {
        if (this.refreshTimer) {
            clearTimeout(this.refreshTimer);
        }

        const accessToken = this.getAccessToken();
        if (!accessToken) return;

        try {
            const tokenData = this.parseJWT(accessToken);
            const expiresAt = tokenData.exp * 1000; // Convert to milliseconds
            const now = Date.now();
            const timeUntilRefresh = expiresAt - now - this.refreshBuffer;

            if (timeUntilRefresh > 0) {
                this.refreshTimer = setTimeout(() => {
                    console.log('🔄 Proactive token refresh...');
                    this.attemptTokenRefresh();
                }, timeUntilRefresh);
                
                const refreshDate = new Date(now + timeUntilRefresh);
                console.log(`⏰ Next token refresh scheduled for: ${refreshDate.toLocaleTimeString()}`);
            } else {
                // Token is about to expire or already expired, refresh immediately
                console.log('⚠️ Token expired or about to expire, refreshing now...');
                this.attemptTokenRefresh();
            }
        } catch (error) {
            console.error('Error scheduling token refresh:', error);
        }
    }

    /**
     * Establece los tokens iniciales (llamar después del login)
     */
    setTokens(accessToken, refreshToken) {
        this.setAccessToken(accessToken);
        this.setRefreshToken(refreshToken);
        this.scheduleTokenRefresh();
    }

    /**
     * Limpia todos los tokens (llamar en logout)
     */
    clearTokens() {
        localStorage.removeItem('authToken');
        localStorage.removeItem('refreshToken');
        
        if (this.refreshTimer) {
            clearTimeout(this.refreshTimer);
            this.refreshTimer = null;
        }
        
        console.log('🚪 Tokens cleared');
    }

    /**
     * Verifica si el usuario está autenticado
     */
    isAuthenticated() {
        const accessToken = this.getAccessToken();
        if (!accessToken) return false;

        try {
            const tokenData = this.parseJWT(accessToken);
            return tokenData.exp * 1000 > Date.now();
        } catch {
            return false;
        }
    }

    // Métodos de utilidad
    getAccessToken() {
        return localStorage.getItem('authToken');
    }

    setAccessToken(token) {
        localStorage.setItem('authToken', token);
    }

    getRefreshToken() {
        return localStorage.getItem('refreshToken');
    }

    setRefreshToken(token) {
        localStorage.setItem('refreshToken', token);
    }

    parseJWT(token) {
        const base64Url = token.split('.')[1];
        const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
        const jsonPayload = decodeURIComponent(atob(base64).split('').map(function(c) {
            return '%' + ('00' + c.charCodeAt(0).toString(16)).slice(-2);
        }).join(''));
        return JSON.parse(jsonPayload);
    }

    isAuthEndpoint(url) {
        const authPaths = ['/auth/', '/api/token/', '/health', '/swagger'];
        return authPaths.some(path => url.includes(path));
    }
}

// Inicializar el servicio automáticamente
window.AutoRefreshTokenService = AutoRefreshTokenService;

// Configuración por defecto
window.tokenService = new AutoRefreshTokenService({
    onTokenRefresh: (tokens) => {
        console.log('🔄 Tokens refreshed:', { hasAccess: !!tokens.accessToken, hasRefresh: !!tokens.refreshToken });
        
        // Opcional: Notificar a otros componentes
        window.dispatchEvent(new CustomEvent('tokensRefreshed', { detail: tokens }));
    },
    onAuthRequired: () => {
        console.log('🔒 Authentication required');
        
        // Opcional: Redirigir al login
        window.dispatchEvent(new CustomEvent('authRequired'));
    }
});

console.log('✅ Auto Refresh Token Service initialized');