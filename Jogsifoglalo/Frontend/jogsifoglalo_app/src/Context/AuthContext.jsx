import { createContext, useState, useEffect, useContext } from 'react';
import axios from 'axios';

const AuthContext = createContext({ user: null, token: null, loading: false });

export const useAuth = () => useContext(AuthContext);

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [token, setToken] = useState(localStorage.getItem('token'));
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        const fetchUser = async () => {
            if (!token) {
                setLoading(false);
                return;
            }
            try {
                const res = await axios.get('https://localhost:7200/api/Users/me', {
                    headers: { Authorization: `Bearer ${token}` }
                });
                setUser(res.data);
            } catch (err) {
                console.error("Auth hiba:", err);
                localStorage.removeItem('token');
                setToken(null);
                setUser(null);
            } finally {
                setLoading(false);
            }
        };
        fetchUser();
    }, [token]);

    const login = (newToken) => {
        localStorage.setItem('token', newToken);
        setToken(newToken);
    };

    const logout = () => {
        localStorage.removeItem('token');
        setToken(null);
        setUser(null);
    };

    return (
        <AuthContext.Provider value={{ user, token, login, logout, loading, setUser }}>
            {!loading && children} 
        </AuthContext.Provider>
    );
};