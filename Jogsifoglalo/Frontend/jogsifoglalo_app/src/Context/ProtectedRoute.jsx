import { Navigate } from 'react-router-dom';
import { useAuth } from './AuthContext';

function ProtectedRoute({ children, requiredRole }) {
    const auth = useAuth();

    if (!auth || auth.loading) {
        return <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', height: '100vh' }}>Betöltés...</div>;
    }

    const { user, token } = auth;

    if (!token) {
        return <Navigate to="/" replace />;
    }

    if (requiredRole && user?.szerepkor !== requiredRole) {
        return <Navigate to="/dashboard" replace />;
    }

    return children;
}

export default ProtectedRoute;