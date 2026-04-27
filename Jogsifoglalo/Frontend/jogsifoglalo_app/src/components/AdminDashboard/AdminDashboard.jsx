import { useState, useEffect } from 'react';
import './AdminDashboard.css';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import Sidebar from '../Sidebar/Sidebar';
import Swal from 'sweetalert2'; 

function AdminDashboard() {
    const navigate = useNavigate();
    const [token] = useState(localStorage.getItem('token'));
    
    const [activeTab, setActiveTab] = useState('users'); 
    const [userSubTab, setUserSubTab] = useState('all'); 
    const [appSubTab, setAppSubTab] = useState('all');
    
    const [searchTerm, setSearchTerm] = useState(""); 
    const [sortOrder, setSortOrder] = useState('idorendi');
    
    const [users, setUsers] = useState([]);
    const [appointments, setAppointments] = useState([]);
    const [stats, setStats] = useState({});
    
    const [error, setError] = useState("");
    const [isLoading, setIsLoading] = useState(true); 

    const [editingUser, setEditingUser] = useState(null);
    const [isAuthorized, setIsAuthorized] = useState(false);

    useEffect(() => {
        const verifyAdminAccess = async () => {
            if (!token) {
                navigate('/');
                return;
            }
            try {
                const res = await axios.get('https://localhost:7200/api/Users/me', {
                    headers: { Authorization: `Bearer ${token}` }
                });

                if (res.data.szerepkor !== 'admin') {
                    Swal.fire('Hiba!', 'Nincs jogosultságod ehhez a felülethez!', 'error');
                    navigate('/dashboard');
                } else {
                    setIsAuthorized(true);
                }
            } catch (err) {
                navigate('/');
            }
        };
        verifyAdminAccess();
    }, [token, navigate]);

    useEffect(() => {
        if (isAuthorized) {
            fetchData();
        }
    }, [activeTab, isAuthorized]);

    const fetchData = async () => {
        setError("");
        setIsLoading(true);
        try {
            const config = { headers: { Authorization: `Bearer ${token}` } };
            
            if (activeTab === 'users') {
                const usersRes = await axios.get('https://localhost:7200/api/Users/all', config);
                const instRes = await axios.get('https://localhost:7200/api/Instructors', config);
                
                const mappedUsers = usersRes.data.map(user => {
                    if (user.szerepkor === 'oktato') {
                        const instructorData = instRes.data.find(inst => inst.email === user.email);
                        return { 
                            ...user, 
                            oktato_Id: instructorData?.oktato_Id,
                            kategoria_Kod: instructorData?.kategoriak?.[0] || 'B'
                        };
                    }
                    return user;
                });
                setUsers(mappedUsers);

            } else if (activeTab === 'appointments') {
                const res = await axios.get('https://localhost:7200/api/Appointments/all', config);
                setAppointments(res.data);

            } else if (activeTab === 'stats') {
                const res = await axios.get('https://localhost:7200/api/Admin/stats', config);
                setStats(res.data);
                
                const appRes = await axios.get('https://localhost:7200/api/Appointments/all', config);
                setAppointments(appRes.data);
            }
        } catch (err) {
            setError("Hiba az adatok betöltésekor.");
        } finally {
            setIsLoading(false);
        }
    };

    const filteredUsers = users.filter(u => {
        const matchesTab = userSubTab === 'all' || u.szerepkor === userSubTab;
        const matchesSearch = u.nev.toLowerCase().includes(searchTerm.toLowerCase()) || 
                              u.email.toLowerCase().includes(searchTerm.toLowerCase());
        return matchesTab && matchesSearch;
    });

    const getFilteredAndSortedAppointments = () => {
        let filtered = [...appointments];
        
        if (appSubTab !== 'all') {
            filtered = filtered.filter(a => a.allapot?.toLowerCase() === appSubTab);
        }

        return filtered.sort((a, b) => {
            if (sortOrder === 'foglalt') {
                if (a.allapot === 'foglalt' && b.allapot !== 'foglalt') return -1;
                if (a.allapot !== 'foglalt' && b.allapot === 'foglalt') return 1;
            } else if (sortOrder === 'szabad') {
                if (a.allapot === 'szabad' && b.allapot !== 'szabad') return -1;
                if (a.allapot !== 'szabad' && b.allapot === 'szabad') return 1;
            }
            return new Date(a.kezdes_Dt) - new Date(b.kezdes_Dt);
        });
    };

    const calculateAdvancedStats = () => {
        const completed = appointments.filter(a => a.allapot === 'teljesitve');
        const tanuloMap = {};
        const oktatoMap = {};

        completed.forEach(a => {
            if (a.tanulo_Nev) {
                tanuloMap[a.tanulo_Nev] = (tanuloMap[a.tanulo_Nev] || 0) + 1;
            }
            if (a.oktato_Nev) {
                oktatoMap[a.oktato_Nev] = (oktatoMap[a.oktato_Nev] || 0) + 1;
            }
        });

        return {
            tanulok: Object.entries(tanuloMap).map(([nev, count]) => ({ nev, count })),
            oktatok: Object.entries(oktatoMap).map(([nev, count]) => ({ nev, count }))
        };
    };

    const handleResetPassword = async (userId) => {
        const { value: newPassword } = await Swal.fire({
            title: 'Új jelszó beállítása',
            input: 'text',
            inputLabel: 'Adj meg egy új jelszót a felhasználónak',
            inputPlaceholder: 'Ide írd az új jelszót...',
            showCancelButton: true,
            confirmButtonText: 'Módosítás',
            cancelButtonText: 'Mégse'
        });

        if (newPassword) {
            try {
                await axios.patch(`https://localhost:7200/api/Admin/users/${userId}/reset-password`, 
                JSON.stringify(newPassword), {
                    headers: { 
                        Authorization: `Bearer ${token}`,
                        'Content-Type': 'application/json'
                    }   
                });
                Swal.fire('Siker!', 'A jelszó sikeresen módosítva.', 'success');
            } catch (err) {
                Swal.fire('Hiba!', 'Nem sikerült a jelszó módosítása.', 'error');
            }
        }
    };

    const handleDeleteUser = async (id) => {
    console.log("Törölni kívánt ID:", id); 

    if (id === undefined || id === null) {
        Swal.fire('Kódolási hiba!', 'Az ID undefined! Ellenőrizd a JSX-ben, hogy u.felhasznalo_Id vagy u.Felhasznalo_Id a helyes név!', 'error');
        return;
    }

    const result = await Swal.fire({
        title: 'Biztosan törlöd?',
        text: "Ez a művelet végleges, minden adat elveszik!",
        icon: 'warning',
        showCancelButton: true,
        confirmButtonColor: '#dc3545',
        confirmButtonText: 'Igen, törlöm!',
        cancelButtonText: 'Mégse'
    });

    if (result.isConfirmed) {
        try {
            await axios.delete(`https://localhost:7200/api/Admin/users/${id}`, {
                headers: { Authorization: `Bearer ${token}` }
            });
            
            await fetchData(); 
            Swal.fire('Törölve!', 'A felhasználó eltávolítva.', 'success');
            
        } catch (err) {
            console.error("Részletes hiba a szervertől:", err.response);
            
            
            const errorMsg = err.response?.data?.message 
                          || err.response?.data 
                          || 'Az adatbázis megtagadta a törlést (valószínűleg a felhasználóhoz még tartozik időpont vagy fizetés).';
                          
            Swal.fire('Hiba a törlésnél!', errorMsg, 'error');
        }
    }
};

    const handleDeleteAppointment = async (id) => {
        const result = await Swal.fire({
            title: 'Biztosan törlöd az időpontot?',
            text: "A törlés után az időpont végleg eltűnik a rendszerből!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#dc3545',
            confirmButtonText: 'Igen, törlöm!',
            cancelButtonText: 'Mégse'
        });

        if (result.isConfirmed) {
            try {
                await axios.delete(`https://localhost:7200/api/Admin/appointments/${id}`, {
                    headers: { Authorization: `Bearer ${token}` }
                });
                fetchData();
                Swal.fire('Törölve!', 'Az időpont eltávolítva.', 'success');
            } catch (err) {
                Swal.fire('Hiba!', 'A törlés nem sikerült.', 'error');
            }
        }
    };

    const handleSaveUser = async (e) => {
        e.preventDefault();
        try {
            const config = { headers: { Authorization: `Bearer ${token}` } };
            
            await axios.put(`https://localhost:7200/api/Admin/users/${editingUser.felhasznalo_Id}`, {
                Nev: editingUser.nev, 
                Email: editingUser.email,
                Telefonszam: editingUser.telefonszam || "06000000000",
                Cim: editingUser.cim || "Ismeretlen",                  
                Szerepkor: editingUser.szerepkor,
                Szerepkor_Nev: editingUser.szerepkor 
            }, config);
            
            setEditingUser(null);
            fetchData();
            Swal.fire('Siker!', 'A felhasználó adatai sikeresen frissítve lettek.', 'success');
        } catch (err) {
            console.error("Mentési hiba:", err.response);
            let errorDetails = "";
            if (err.response?.data?.errors) {
                errorDetails = Object.values(err.response.data.errors).flat().join('\n');
            }
            Swal.fire('Hiba!', `A mentés nem sikerült.\n${errorDetails}`, 'error');
        }
    };

    if (!isAuthorized) {
        return (
            <div className="page-container">
                <Sidebar />
                <div className="content-area" style={{ display: 'flex', justifyContent: 'center', alignItems: 'center' }}>
                    <h3>Jogosultság ellenőrzése...</h3>
                </div>
            </div>
        );
    }

    const advStats = calculateAdvancedStats();

    return (
        <div className="page-container">
            <Sidebar />
            <div className="content-area fade-in">
                <div className="header-section">
                    <h1>Adminisztráció</h1>
                    <p>Rendszeradatok kezelése és statisztikák.</p>
                </div>

                <div className="admin-tabs">
                    <button className={activeTab === 'users' ? 'active' : ''} onClick={() => setActiveTab('users')}>Felhasználók</button>
                    <button className={activeTab === 'appointments' ? 'active' : ''} onClick={() => setActiveTab('appointments')}>Időpontok</button>
                    <button className={activeTab === 'stats' ? 'active' : ''} onClick={() => setActiveTab('stats')}>Statisztikák</button>
                </div>

                <div className="dash-card admin-card">
                    {error && <div className="regist-error">{error}</div>}

                    {activeTab === 'users' && (
                        <>
                            <div className="admin-controls" style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '20px', gap: '20px' }}>
                                <div className="admin-sub-tabs" style={{ border: 'none', margin: 0 }}>
                                    <button className={userSubTab === 'all' ? 'active' : ''} onClick={() => setUserSubTab('all')}>Mindenki</button>
                                    <button className={userSubTab === 'tanulo' ? 'active' : ''} onClick={() => setUserSubTab('tanulo')}>Tanulók</button>
                                    <button className={userSubTab === 'oktato' ? 'active' : ''} onClick={() => setUserSubTab('oktato')}>Oktatók</button>
                                </div>
                                <input 
                                    type="text" 
                                    placeholder="Keresés név vagy email alapján..." 
                                    className="admin-search-input"
                                    value={searchTerm}
                                    onChange={(e) => setSearchTerm(e.target.value)}
                                    style={{ flex: 1, padding: '10px 15px', borderRadius: '8px', border: '1px solid #ddd' }}
                                />
                            </div>

                            <div className="table-responsive">
                                <table className="admin-table">
                                    <thead>
                                        <tr>
                                            <th>ID</th>
                                            <th>Név</th>
                                            <th>Email</th>
                                            <th>Szerepkör</th>
                                            <th>Művelet</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {isLoading ? (
                                            [1,2,3,4,5].map(i => (
                                                <tr key={i} className="skeleton-row">
                                                    <td colSpan="5" style={{ height: '50px', background: 'linear-gradient(90deg, #f0f0f0 25%, #e0e0e0 50%, #f0f0f0 75%)', backgroundSize: '200% 100%', animation: 'pulse 1.5s infinite' }}></td>
                                                </tr>
                                            ))
                                        ) : (
                                            filteredUsers.map(u => (
                                                <tr key={u.felhasznalo_Id}>
                                                    <td>{u.felhasznalo_Id}</td>
                                                    <td><strong>{u.nev}</strong></td>
                                                    <td>{u.email}</td>
                                                    <td>
                                                        <span className={`badge-role role-${u.szerepkor}`}>
                                                            {u.szerepkor} {u.szerepkor === 'oktato' && `(${u.kategoria_Kod || '?'})`}
                                                        </span>
                                                    </td>
                                                    <td>
                                                        <button className="btn-edit" onClick={() => setEditingUser({...u})}>Szerkesztés</button>
                                                        <button className="btn-edit" style={{background: '#6c757d', color: 'white', marginLeft: '5px', marginRight: '5px'}} onClick={() => handleResetPassword(u.felhasznalo_Id)}>Jelszó megváltoztatása</button>
                                                        <button className="btn-delete" onClick={() => handleDeleteUser(u.felhasznalo_Id)}>Törlés</button>
                                                    </td>
                                                </tr>
                                            ))
                                        )}
                                    </tbody>
                                </table>
                            </div>
                        </>
                    )}

                    {activeTab === 'appointments' && (
                        <>
                            <div className="admin-controls" style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '20px', gap: '20px' }}>
                                <div className="admin-sub-tabs" style={{ border: 'none', margin: 0 }}>
                                    <button className={appSubTab === 'all' ? 'active' : ''} onClick={() => setAppSubTab('all')}>Összes óra</button>
                                    <button className={appSubTab === 'szabad' ? 'active' : ''} onClick={() => setAppSubTab('szabad')}>Szabad</button>
                                    <button className={appSubTab === 'foglalt' ? 'active' : ''} onClick={() => setAppSubTab('foglalt')}>Foglalt</button>
                                    <button className={appSubTab === 'teljesitve' ? 'active' : ''} onClick={() => setAppSubTab('teljesitve')}>Teljesített</button>
                                </div>

                                <select 
                                    value={sortOrder} 
                                    onChange={(e) => setSortOrder(e.target.value)}
                                    style={{ padding: '8px 15px', borderRadius: '8px', border: '1px solid #ddd', height: 'fit-content' }}
                                >
                                    <option value="idorendi">Időrendi sorrend</option>
                                    <option value="foglalt">Foglaltak előre</option>
                                    <option value="szabad">Szabadok előre</option>
                                </select>
                            </div>
                            <div className="table-responsive">
                                <table className="admin-table">
                                    <thead>
                                        <tr>
                                            <th>ID</th>
                                            <th>Dátum</th>
                                            <th>Oktató</th>
                                            <th>Tanuló</th>
                                            <th>Kategória</th>
                                            <th>Állapot</th>
                                            <th>Műveletek</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {isLoading ? (
                                            <tr><td colSpan="7" style={{textAlign: 'center'}}>Betöltés...</td></tr>
                                        ) : getFilteredAndSortedAppointments().length > 0 ? (
                                            getFilteredAndSortedAppointments().map(app => (
                                                <tr key={app.idopont_Id}>
                                                    <td>{app.idopont_Id}</td>
                                                    <td><strong>{new Date(app.kezdes_Dt).toLocaleString('hu-HU', { year: 'numeric', month: 'short', day: 'numeric', hour: '2-digit', minute:'2-digit' })}</strong></td>
                                                    <td>{app.oktato_Nev || 'Ismeretlen'}</td>
                                                    <td>{app.tanulo_Nev || <span style={{color: '#38a169', fontWeight: 'bold'}}>SZABAD</span>}</td>
                                                    <td>{app.kategoria_Kod || '?'}</td>
                                                    <td>
                                                        <span className={`status-badge status-${app.allapot?.toLowerCase() || 'szabad'}`}>
                                                            {app.allapot?.toUpperCase()}
                                                        </span>
                                                    </td>
                                                    <td>
                                                        <button className="btn-delete" onClick={() => handleDeleteAppointment(app.idopont_Id)}>Törlés</button>
                                                    </td>
                                                </tr>
                                            ))
                                        ) : (
                                            <tr><td colSpan="7" style={{textAlign: 'center', padding: '40px', color: '#a0aec0', fontStyle: 'italic'}}>Nincs a szűrésnek megfelelő időpont.</td></tr>
                                        )}
                                    </tbody>
                                </table>
                            </div>
                        </>
                    )}

                    {activeTab === 'stats' && (
                        <div className="stats-container">
                            <h3 style={{marginBottom: '20px', color: '#2d3748'}}>Rendszer Áttekintés</h3>
                            
                            {isLoading ? (
                                <div style={{textAlign: 'center', padding: '30px', color: '#718096'}}>Adatok betöltése...</div>
                            ) : (
                                <div style={{ display: 'flex', gap: '20px', flexWrap: 'wrap', marginBottom: '40px' }}>
                                    <div style={{ flex: 1, minWidth: '200px', background: '#f8fafc', padding: '25px', borderRadius: '12px', textAlign: 'center', borderTop: '4px solid #1877f2', boxShadow: '0 2px 5px rgba(0,0,0,0.05)' }}>
                                        <h4 style={{ margin: '0 0 10px 0', color: '#718096', fontSize: '13px', textTransform: 'uppercase', letterSpacing: '0.5px' }}>Összes Felhasználó</h4>
                                        <strong style={{ fontSize: '28px', color: '#2d3748' }}>{stats.osszesFelhasznalo || 0} fő</strong>
                                    </div>

                                    <div style={{ flex: 1, minWidth: '200px', background: '#f8fafc', padding: '25px', borderRadius: '12px', textAlign: 'center', borderTop: '4px solid #38a169', boxShadow: '0 2px 5px rgba(0,0,0,0.05)' }}>
                                        <h4 style={{ margin: '0 0 10px 0', color: '#718096', fontSize: '13px', textTransform: 'uppercase', letterSpacing: '0.5px' }}>Aktív Oktatók</h4>
                                        <strong style={{ fontSize: '28px', color: '#2d3748' }}>{stats.aktivOktatokSzama || 0} fő</strong>
                                    </div>

                                    <div style={{ flex: 1, minWidth: '200px', background: '#f8fafc', padding: '25px', borderRadius: '12px', textAlign: 'center', borderTop: '4px solid #d69e2e', boxShadow: '0 2px 5px rgba(0,0,0,0.05)' }}>
                                        <h4 style={{ margin: '0 0 10px 0', color: '#718096', fontSize: '13px', textTransform: 'uppercase', letterSpacing: '0.5px' }}>Összes Bevétel</h4>
                                        <strong style={{ fontSize: '28px', color: '#d69e2e' }}>{(stats.osszesBevetel || 0).toLocaleString('hu-HU')} Ft</strong>
                                    </div>

                                    <div style={{ flex: 1, minWidth: '200px', background: '#f8fafc', padding: '25px', borderRadius: '12px', textAlign: 'center', borderTop: '4px solid #e53e3e', boxShadow: '0 2px 5px rgba(0,0,0,0.05)' }}>
                                        <h4 style={{ margin: '0 0 10px 0', color: '#718096', fontSize: '13px', textTransform: 'uppercase', letterSpacing: '0.5px' }}>Mai Időpontok</h4>
                                        <strong style={{ fontSize: '28px', color: '#2d3748' }}>{stats.maiIdopontokSzama || 0} db</strong>
                                    </div>
                                </div>
                            )}

                            <div style={{ display: 'flex', gap: '20px', flexWrap: 'wrap' }}>
                                <div style={{ flex: 1, minWidth: '300px', background: '#f8fafc', padding: '20px', borderRadius: '8px', border: '1px solid #e2e8f0' }}>
                                    <h4 style={{marginTop: 0, borderBottom: '1px solid #e2e8f0', paddingBottom: '10px', color: '#2d3748'}}>Levezetett órák (Oktatók szerint)</h4>
                                    {advStats.oktatok.length > 0 ? advStats.oktatok.map(o => (
                                        <div key={o.nev} style={{ display: 'flex', justifyContent: 'space-between', padding: '8px 0', borderBottom: '1px dashed #cbd5e0' }}>
                                            <span>{o.nev}:</span> <strong>{o.count} óra</strong>
                                        </div>
                                    )) : <p style={{color: '#a0aec0', fontStyle: 'italic', fontSize: '14px'}}>Még nincsenek teljesített órák a rendszerben.</p>}
                                </div>

                                <div style={{ flex: 1, minWidth: '300px', background: '#f8fafc', padding: '20px', borderRadius: '8px', border: '1px solid #e2e8f0' }}>
                                    <h4 style={{marginTop: 0, borderBottom: '1px solid #e2e8f0', paddingBottom: '10px', color: '#2d3748'}}>Levezetett órák (Diákok szerint)</h4>
                                    {advStats.tanulok.length > 0 ? advStats.tanulok.map(t => (
                                        <div key={t.nev} style={{ display: 'flex', justifyContent: 'space-between', padding: '8px 0', borderBottom: '1px dashed #cbd5e0' }}>
                                            <span>{t.nev}:</span> <strong>{t.count} óra</strong>
                                        </div>
                                    )) : <p style={{color: '#a0aec0', fontStyle: 'italic', fontSize: '14px'}}>Még nincsenek teljesített órák a rendszerben.</p>}
                                </div>
                            </div>
                        </div>
                    )}

                </div>
            </div>

            {editingUser && (
                <div className="modal-overlay">
                    <div className="modal-content slide-up">
                        <h3>Felhasználó szerkesztése</h3>
                        <form onSubmit={handleSaveUser} className="admin-edit-form">
                            
                            <label>Név</label>
                            <input type="text" value={editingUser.nev} onChange={e => setEditingUser({...editingUser, nev: e.target.value})} required />
                            
                            <label>Email</label>
                            <input type="email" value={editingUser.email} onChange={e => setEditingUser({...editingUser, email: e.target.value})} required />
                            
                            <label>Telefonszám</label>
                            <input type="text" value={editingUser.telefonszam || ''} onChange={e => setEditingUser({...editingUser, telefonszam: e.target.value})} required />

                            <label>Lakcím</label>
                            <input type="text" value={editingUser.cim || ''} onChange={e => setEditingUser({...editingUser, cim: e.target.value})} required />

                            <p style={{fontSize: '12px', color: '#a0aec0', marginTop: '15px', fontStyle: 'italic'}}>
                                *A jogosultságok (szerepkör, oktatott kategória) módosítása biztonsági okokból a felületről nem engedélyezett.
                            </p>

                            <div className="modal-actions">
                                <button type="button" className="btn-cancel" onClick={() => setEditingUser(null)}>Mégse</button>
                                <button type="submit" className="btn-save">Mentés</button>
                            </div>
                        </form>
                    </div>
                </div>
            )}
        </div>
    );
}

export default AdminDashboard;