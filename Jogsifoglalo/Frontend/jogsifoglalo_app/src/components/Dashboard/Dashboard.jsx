import { useState, useEffect } from 'react';
import './Dashboard.css';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import Swal from 'sweetalert2'; 
import Sidebar from '../Sidebar/Sidebar';
import { useAuth } from "../../Context/AuthContext";

function Dashboard() {
    const navigate = useNavigate();
    const { user, token } = useAuth(); 
    const [lessons, setLessons] = useState([]);
    const [error, setError] = useState("");
    const [filterType, setFilterType] = useState('chrono');

    useEffect(() => {
        if (user && user.szerepkor !== 'admin') {
            fetchDashboardData();
        }
    }, [user, token]);

    const fetchDashboardData = async () => {
        try {
            const isOktato = user.szerepkor === 'oktato';
            const endpoint = isOktato ? 'my-schedule' : 'my-bookings';

            const lessonsResponse = await axios.get(`https://localhost:7200/api/Appointments/${endpoint}`, {
                headers: { Authorization: `Bearer ${token}` }
            });
            setLessons(lessonsResponse.data);
            setError(""); 
            
        } catch (err) {
            if (err.response?.status === 404) {
                setLessons([]);
            } else {
                setError("Hiba történt az adatok betöltésekor.");
            }
        }
    };


    const getFilteredLessons = () => {
        let filtered = [...lessons];
        
        if (filterType === 'teljesitve') {
            return filtered.filter(l => l.allapot?.toLowerCase() === 'teljesitve');
        } else if (filterType === 'szabad') {
            return filtered.filter(l => l.allapot?.toLowerCase() === 'szabad');
        } else if (filterType === 'foglalt') {
            return filtered.filter(l => l.allapot?.toLowerCase() === 'foglalt');
        }
        
        return filtered.sort((a, b) => new Date(a.kezdes_Dt) - new Date(b.kezdes_Dt));
    };

    const handleCancel = async (idopontId) => {
        const isOktato = user.szerepkor === 'oktato';
        const result = await Swal.fire({
            title: isOktato ? 'Biztosan törlöd ezt az órát?' : 'Biztosan lemondod az időpontot?',
            text: "Ezt a műveletet később nem tudod visszavonni!",
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#d33',
            cancelButtonColor: '#a0aec0',
            confirmButtonText: isOktato ? 'Igen, törlöm!' : 'Igen, lemondom!',
            cancelButtonText: 'Mégse'
        });

        if (result.isConfirmed) {
            try {
                if (user.szerepkor === 'tanulo') {
                    await axios.put(`https://localhost:7200/api/Appointments/cancel/${idopontId}`, {}, {
                        headers: { Authorization: `Bearer ${token}` }
                    });
                } else if (user.szerepkor === 'oktato') {
                    await axios.delete(`https://localhost:7200/api/Appointments/${idopontId}`, {
                        headers: { Authorization: `Bearer ${token}` }
                    });
                }
                await fetchDashboardData();
                Swal.fire('Sikeres!', 'A műveletet végrehajtottuk.', 'success'); 
            } catch (err) {
                Swal.fire('Hiba!', err.response?.data || "Hiba történt a művelet során.", 'error'); 
            }
        }
    };

    const handleComplete = async (idopontId) => {
        try {
            await axios.put(`https://localhost:7200/api/Appointments/complete/${idopontId}`, {}, {
                headers: { Authorization: `Bearer ${token}` }
            });
            Swal.fire('Siker!', 'Az órát teljesítettnek jelölted.', 'success');
            fetchDashboardData();
        } catch (err) {
            Swal.fire('Hiba!', 'Nem sikerült lezárni az órát.', 'error');
        }
    };

    const isPastLesson = (startDate, durationMinutes) => {
        if (!startDate) return false;
        const end = new Date(new Date(startDate).getTime() + (durationMinutes || 50) * 60000);
        return new Date() > end;
    };

    if (!user) return <div className="page-container"><Sidebar /></div>;

    const displayLessons = getFilteredLessons();

    return (
        <div className="page-container">
            <Sidebar />
            <div className="content-area fade-in">
                <div className="header-section">
                    <h1>Üdvözlünk, {user.nev}! </h1>
                    <p>Itt áttekintheted a profilodat és a teendőidet.</p>
                </div>

                <div className="dash-card">
                    {user.szerepkor === 'admin' ? (
                        <div className="admin-welcome-section" style={{ textAlign: 'center', padding: '40px 20px' }}>
                            <div style={{ fontSize: '40px', marginBottom: '15px' }}>🛡️</div>
                            <h3>Rendszergazdai Áttekintő</h3>
                            <button onClick={() => navigate('/admin')} className="btn-save" style={{padding: '12px 25px'}}>Irány az Adminisztráció →</button>
                        </div>
                    ) : (
                        <>
                            <div className="dashboard-controls">
                                <h3>{user.szerepkor === 'oktato' ? 'Oktatási órarend' : 'Vezetési óráim'}</h3>
                                
                                <div className="filter-buttons">
                                    <button className={filterType === 'chrono' ? 'active' : ''} onClick={() => setFilterType('chrono')}>Időrendi</button>
                                    
                                    {user.szerepkor !== 'tanulo' && (
                                        <button className={filterType === 'szabad' ? 'active' : ''} onClick={() => setFilterType('szabad')}>Szabad</button>
                                    )}
                                    
                                    <button className={filterType === 'foglalt' ? 'active' : ''} onClick={() => setFilterType('foglalt')}>Foglalt</button>
                                    <button className={filterType === 'teljesitve' ? 'active' : ''} onClick={() => setFilterType('teljesitve')}>Teljesített</button>
                                </div>
                            </div>
                            
                            {error && <div className="regist-error" style={{ color: 'red', marginBottom: '10px' }}>{error}</div>}

                            <div className="table-responsive">
                                <table className="admin-table">
                                    <thead>
                                        <tr>
                                            <th>Időpont</th>
                                            <th>{user.szerepkor === 'oktato' ? 'Tanuló' : 'Oktató'}</th>
                                            <th>Kategória</th>
                                            <th>Állapot</th>
                                            <th>Műveletek</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        {displayLessons.length > 0 ? (
                                            displayLessons.map((lesson) => {
                                                const dateObj = new Date(lesson.kezdes_Dt);
                                                const currentStatus = lesson.allapot?.toLowerCase();

                                                return (
                                                    <tr key={lesson.idopont_Id} className={currentStatus === 'teljesitve' ? 'row-completed' : ''}>
                                                        <td>
                                                            <strong>{dateObj.toLocaleDateString('hu-HU', { month: 'short', day: 'numeric' })}</strong>
                                                            <br />
                                                            <span className="time-text">{dateObj.toLocaleTimeString('hu-HU', { hour: '2-digit', minute:'2-digit' })}</span>
                                                        </td>
                                                        <td>{user.szerepkor === 'oktato' ? (lesson.tanulo_Nev || '-') : lesson.oktato_Nev}</td>
                                                        <td><span className="cat-badge-small">{lesson.kategoria_Kod || 'N/A'}</span></td>
                                                        <td>
                                                            <span className={`status-badge status-${currentStatus}`}>
                                                                {lesson.allapot?.toUpperCase()}
                                                            </span>
                                                        </td>
                                                        <td>
                                                            <div className="action-cell">
                                                                {user.szerepkor === 'oktato' && currentStatus === 'foglalt' && isPastLesson(lesson.kezdes_Dt, lesson.idotartam) && (
                                                                    <button className="btn-save-small" onClick={() => handleComplete(lesson.idopont_Id)}>Igazolás</button>
                                                                )}
                                                                {((user.szerepkor === 'tanulo' && currentStatus === 'foglalt') || 
                                                                  (user.szerepkor === 'oktato' && currentStatus !== 'teljesitve')) && (
                                                                    <button className="btn-cancel-small" onClick={() => handleCancel(lesson.idopont_Id)}>
                                                                        {user.szerepkor === 'oktato' ? 'Törlés' : 'Lemondás'}
                                                                    </button>
                                                                )}
                                                            </div>
                                                        </td>
                                                    </tr>
                                                );
                                            })
                                        ) : (
                                            <tr>
                                                <td colSpan="5" className="empty-row">Nincs megjeleníthető óra ebben a kategóriában.</td>
                                            </tr>
                                        )}
                                    </tbody>
                                </table>
                            </div>
                        </>
                    )}
                </div>
            </div>
        </div>
    );
}

export default Dashboard;