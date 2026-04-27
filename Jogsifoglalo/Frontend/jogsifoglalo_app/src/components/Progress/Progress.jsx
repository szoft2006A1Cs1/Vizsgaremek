import { useState, useEffect } from 'react';
import axios from 'axios';
import Sidebar from '../Sidebar/Sidebar';
import { useAuth } from "../../Context/AuthContext";
import './Progress.css';

function Progress() {
    const { user, token } = useAuth();
    const [history, setHistory] = useState([]);
    const [isLoading, setIsLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        if (user) {
            fetchHistory();
        }
    }, [user, token]);

    const fetchHistory = async () => {
        try {
            setIsLoading(true);
            
            const endpoint = user.szerepkor === 'oktato' ? 'my-schedule' : 'my-bookings';
            
            const response = await axios.get(`https://localhost:7200/api/Appointments/${endpoint}`, {
                headers: { Authorization: `Bearer ${token}` }
            });

            const completedLessons = response.data.filter(lesson => lesson.allapot?.toLowerCase() === 'teljesitve');
            setHistory(completedLessons);
            setError("");
        } catch (err) {
            setError("Hiba történt az adatok betöltésekor.");
        } finally {
            setIsLoading(false);
        }
    };

    const totalMinutes = history.reduce((sum, lesson) => sum + (lesson.idotartam || 0), 0);
    const totalHours = Math.floor(totalMinutes / 60);
    const remainingMinutes = totalMinutes % 60;
    const totalMoney = history.reduce((sum, lesson) => sum + (lesson.ar || 0), 0);

    if (!user) return <div className="page-container"><Sidebar /></div>;

    const isOktato = user.szerepkor === 'oktato';

    return (
        <div className="page-container">
            <Sidebar />
            <div className="content-area fade-in">
                <div className="header-section">
                    <h1>{isOktato ? 'Oktatói Statisztikáim ' : 'Előrehaladásom '}</h1>
                    <p>{isOktato ? 'Itt láthatod az eddig leadott óráidat és a bevételedet.' : 'Kövesd nyomon, mennyit vezettél eddig és hogyan állsz anyagilag.'}</p>
                </div>

                {error && <div className="regist-error">{error}</div>}

                <div className="progress-stats-container">
                    <div className="progress-stat-card blue">
                        <h3 className="stat-title">Teljesített alkalmak</h3>
                        <div className="stat-value blue">{history.length} db</div>
                    </div>
                    
                    <div className="progress-stat-card green">
                        <h3 className="stat-title">Vezetett idő</h3>
                        <div className="stat-value green">
                            {totalHours} <span style={{fontSize: '1rem'}}>óra</span> {remainingMinutes} <span style={{fontSize: '1rem'}}>perc</span>
                        </div>
                    </div>

                    <div className="progress-stat-card yellow">
                        <h3 className="stat-title">{isOktato ? 'Összes Bevétel' : 'Összes Költés'}</h3>
                        <div className="stat-value yellow">
                            {totalMoney.toLocaleString('hu-HU')} <span style={{fontSize: '1.2rem'}}>Ft</span>
                        </div>
                    </div>
                </div>

                <div className="dash-card progress-history-section">
                    <h3>Eddigi történet</h3>
                    <div className="table-responsive">
                        <table className="admin-table">
                            <thead>
                                <tr>
                                    <th>Dátum</th>
                                    <th>{isOktato ? 'Tanuló neve' : 'Oktató neve'}</th>
                                    <th>Kategória</th>
                                    <th>Időtartam</th>
                                    <th>Ár</th>
                                </tr>
                            </thead>
                            <tbody>
                                {isLoading ? (
                                    <tr><td colSpan="5" style={{ textAlign: 'center', padding: '30px' }}>Adatok betöltése folyamatban...</td></tr>
                                ) : history.length > 0 ? (
                                    history.sort((a, b) => new Date(b.kezdes_Dt) - new Date(a.kezdes_Dt)).map((lesson) => (
                                        <tr key={lesson.idopont_Id}>
                                            <td><strong>{new Date(lesson.kezdes_Dt).toLocaleString('hu-HU', { year: 'numeric', month: 'short', day: 'numeric', hour: '2-digit', minute:'2-digit' })}</strong></td>
                                            <td>{isOktato ? (lesson.tanulo_Nev || 'Ismeretlen') : (lesson.oktato_Nev || 'Ismeretlen')}</td>
                                            
                                            <td>
                                                <span style={{ background: '#f1f5f9', padding: '4px 10px', borderRadius: '6px', fontSize: '13px', fontWeight: 'bold', color: '#475569' }}>
                                                    {lesson.kategoria_Kod || '?'}
                                                </span>
                                            </td>

                                            <td>{lesson.idotartam} perc</td>
                                            <td style={{fontWeight: 'bold', color: '#4a5568'}}>{lesson.ar.toLocaleString('hu-HU')} Ft</td>
                                        </tr>
                                    ))
                                ) : (
                                    <tr>
                                        <td colSpan="5">
                                            <div className="empty-history">
                                                Még nincsenek teljesített óráid a rendszerben.
                                            </div>
                                        </td>
                                    </tr>
                                )}
                            </tbody>
                        </table>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Progress;