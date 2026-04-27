import { useState } from 'react';
import { Link, useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from "../../Context/AuthContext";
import './Sidebar.css';

function Sidebar() {
    const navigate = useNavigate();
    const location = useLocation();
    const { user, logout } = useAuth(); 
    
    const [isOpen, setIsOpen] = useState(false); 

    const handleLogout = () => {
        logout(); 
        navigate('/');
    };

    if (!user) return null; 

    const baseUrl = "https://localhost:7200";

    const closeMenu = () => setIsOpen(false);

    const szerepkorMegnevezesek = {
        'tanulo': 'Tanuló',
        'oktato': 'Oktató',
        'admin': 'Adminisztrátor'
    };

    return (
        <>
            <button className="mobile-menu-btn" onClick={() => setIsOpen(!isOpen)}>
                {isOpen ? '✖' : '☰'}
            </button>

            {isOpen && <div className="sidebar-overlay" onClick={closeMenu}></div>}

            <div className={`sidebar ${isOpen ? 'open' : ''}`}>
                <div className="sidebar-logo">
                    <h2>Jogsifoglaló</h2>
                </div>

                <div className="sidebar-menu">
                    <Link to="/dashboard" onClick={closeMenu} className={`menu-item ${location.pathname === '/dashboard' ? 'active' : ''}`}>
                        Áttekintés
                    </Link>

                    {user.szerepkor === 'tanulo' && (
                        <Link to="/booking" onClick={closeMenu} className={`menu-item ${location.pathname === '/booking' ? 'active' : ''}`}>
                            Időpont foglalása
                        </Link>
                    )}

                    {user.szerepkor === 'oktato' && (
                        <Link to="/create-appointment" onClick={closeMenu} className={`menu-item ${location.pathname === '/create-appointment' ? 'active' : ''}`}>
                            Új időpont
                        </Link>
                    )}

                    {(user.szerepkor === 'tanulo' || user.szerepkor === 'oktato') && (
                        <Link to="/progress" onClick={closeMenu} className={`menu-item ${location.pathname === '/progress' ? 'active' : ''}`}>
                            {user.szerepkor === 'tanulo' ? 'Előrehaladás' : 'Statisztikáim'}
                        </Link>
                    )}

                    <Link to="/profile" onClick={closeMenu} className={`menu-item ${location.pathname === '/profile' ? 'active' : ''}`}>
                        Beállítások
                    </Link>

                    {user.szerepkor === 'admin' && (
                        <Link to="/admin" onClick={closeMenu} className={`menu-item ${location.pathname === '/admin' ? 'active' : ''}`}>
                            Adminisztráció
                        </Link>
                    )}

                    <Link to="/forgot-password" onClick={closeMenu} className={`menu-item ${location.pathname === '/forgot-password' ? 'active' : ''}`}>
                        Jelszó módosítása
                    </Link>
                </div>

                <div className="sidebar-footer">
                    <div className="user-info">
                        <div className="user-avatar">
                            {user.profilkep ? (
                                <img 
                                    src={`${baseUrl}${user.profilkep}`} 
                                    alt="Profil" 
                                    style={{ width: '100%', height: '100%', borderRadius: '50%', objectFit: 'cover' }}
                                />
                            ) : (
                                user.nev.charAt(0)
                            )}
                        </div>
                        <div className="user-details">
                            <strong>{user.nev}</strong>
                            <span>{szerepkorMegnevezesek[user.szerepkor] || user.szerepkor}</span>
                        </div>
                    </div>
                    <button className="btn-logout-sidebar" onClick={handleLogout}>Kijelentkezés</button>
                </div>
            </div>
        </>
    );
}

export default Sidebar;