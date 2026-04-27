import { useState } from 'react';
import './CreateAppointment.css';
import { useNavigate, Link } from 'react-router-dom';
import axios from 'axios';
import { useAuth } from '../../Context/AuthContext'; 

function CreateAppointment() {
    const navigate = useNavigate();
    const { token, user } = useAuth();
    
    const [mode, setMode] = useState(null);

    const [oktatId, setOktatId] = useState(user?.oktat_Id || ""); 
    
    const [singleData, setSingleData] = useState({
        kezdes: "",
        idotartam: 50,
        ar: 8000,
        megjegyzes: ""
    });

    const [multiData, setMultiData] = useState([
        { kezdes: "", idotartam: 50, ar: 8000, megjegyzes: "" }
    ]);
    
    const [error, setError] = useState("");
    const [successMsg, setSuccessMsg] = useState("");

    const addAppointmentRow = () => {
        setMultiData([...multiData, { kezdes: "", idotartam: 50, ar: 8000, megjegyzes: "" }]);
    };

    const removeAppointmentRow = (index) => {
        const newData = multiData.filter((_, i) => i !== index);
        setMultiData(newData);
    };

    const handleMultiChange = (index, field, value) => {
        const newData = [...multiData];
        newData[index][field] = value;
        setMultiData(newData);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");
        setSuccessMsg("");

        if (!oktatId) {
            setError("Kérlek válassz egy érvényes kategóriát!");
            return;
        }

        const now = new Date();
        let newApps = [];

        if (mode === 'single') {
            newApps.push({ 
                kezdes: singleData.kezdes, 
                idotartam: parseInt(singleData.idotartam),
                ar: parseInt(singleData.ar),
                megjegyzes: singleData.megjegyzes,
                index: 'Egyetlen'
            });
        } else {
            newApps = multiData.map((row, i) => ({ 
                kezdes: row.kezdes, 
                idotartam: parseInt(row.idotartam),
                ar: parseInt(row.ar),
                megjegyzes: row.megjegyzes,
                index: i + 1
            }));
        }

        for (let i = 0; i < newApps.length; i++) {
            const app = newApps[i];
            if (!app.kezdes) {
                setError(`A(z) ${app.index}. sorban nincs megadva kezdési idő!`);
                return;
            }
            if (new Date(app.kezdes) < now) {
                setError(`A(z) ${app.index}. sor kezdési ideje a múltban van!`);
                return;
            }
        }

        for (let i = 0; i < newApps.length; i++) {
            for (let j = i + 1; j < newApps.length; j++) {
                const start1 = new Date(newApps[i].kezdes).getTime();
                const end1 = start1 + newApps[i].idotartam * 60000;
                const start2 = new Date(newApps[j].kezdes).getTime();
                const end2 = start2 + newApps[j].idotartam * 60000;

                if (start1 < end2 && start2 < end1) {
                    setError(`A megadott időpontok ütköznek egymással az űrlapon! (${newApps[i].index}. és ${newApps[j].index}. sor)`);
                    return;
                }
            }
        }

        try {
            const scheduleRes = await axios.get('https://localhost:7200/api/Appointments/my-schedule', {
                headers: { Authorization: `Bearer ${token}` }
            });
            const existingAppointments = scheduleRes.data;

            for (let i = 0; i < newApps.length; i++) {
                const start1 = new Date(newApps[i].kezdes).getTime();
                const end1 = start1 + newApps[i].idotartam * 60000;

                for (let j = 0; j < existingAppointments.length; j++) {
                    const extApp = existingAppointments[j];
                    const start2 = new Date(extApp.kezdes_Dt).getTime();
                    const end2 = start2 + extApp.idotartam * 60000;

                    if (start1 < end2 && start2 < end1) {
                        const existingDate = new Date(extApp.kezdes_Dt).toLocaleString('hu-HU', { month: 'short', day: 'numeric', hour: '2-digit', minute:'2-digit' });
                        setError(`A(z) ${newApps[i].index}. sor ütközik egy már létező óráddal (${existingDate})!`);
                        return;
                    }
                }
            }

            const apiPromises = newApps.map(app => 
                axios.post('https://localhost:7200/api/Appointments/create', {
                    Kezdes_Dt: app.kezdes,
                    Idotartam: app.idotartam,
                    Ar: app.ar,
                    Oktat_Id: parseInt(oktatId),
                    Megjegyzes: app.megjegyzes
                }, { headers: { Authorization: `Bearer ${token}` } })
            );

            await Promise.all(apiPromises);

            setSuccessMsg("Időpont(ok) sikeresen meghirdetve! Visszairányítunk...");
            setTimeout(() => {
                navigate('/dashboard');
            }, 2000);
            
        } catch (err) {
            setError(err.response?.data || "Hiba történt az időpont(ok) mentése során.");
        }
    };

    return (
        <div className="main-wrapper">
            <div className="split-container create-appt-card">
                <div className="left-side create-appt-bg">
                    <div className="overlay"></div>
                    <div className="left-content fade-in">
                        <h2 className="logo-text">Jogsifoglaló</h2>
                        <div className="hero-text">
                            <h1>Oktatói Vezérlő</h1>
                            <p>Töltsd fel a naptárad, és segíts a tanulóknak megszerezni a jogosítványukat!</p>
                        </div>
                    </div>
                </div>

                <div className="right-side slide-up" style={{ overflowY: 'auto', display: 'block', padding: '40px 20px' }}>
                    <div className="form-box" style={{ maxWidth: mode === 'multi' ? '800px' : '450px', margin: '0 auto' }}>
                        <div className="form-header">
                            <h2>Új időpont(ok) meghirdetése</h2>
                            <p>Oktató: <strong>{user?.nev}</strong></p>
                        </div>

                        {!mode && (
                            <div className="mode-selection" style={{ display: 'flex', flexDirection: 'column', gap: '15px', marginTop: '30px' }}>
                                <button className="btn-reg" onClick={() => setMode('single')}>
                                    Egy időpont rögzítése
                                </button>
                                <button className="btn-reg" style={{ backgroundColor: '#4a5568' }} onClick={() => setMode('multi')}>
                                    Több időpont rögzítése egyszerre
                                </button>
                                <Link to='/dashboard' className="btn-login-link" style={{ textAlign: 'center', marginTop: '15px' }}>Vissza a főoldalra</Link>
                            </div>
                        )}

                        {mode && (
                            <form className="reg-form" onSubmit={handleSubmit}>
                                <div className="input-group-wrapper">
                                    <label>Kategória</label>
                                    <div className="input-field">
                                        <select 
                                            value={oktatId} 
                                            onChange={e => setOktatId(e.target.value)}
                                            required
                                        >
                                            <option value="" disabled>Válassz kategóriát...</option>
                                            <option value="1">A - Motor (ID: 1)</option>
                                            <option value="2">B - Személygépkocsi (ID: 2)</option>
                                            <option value="3">C - Teherautó (ID: 3)</option>
                                        </select>
                                    </div>
                                </div>

                                {mode === 'single' && (
                                    <>
                                        <div className="input-group-wrapper">
                                            <label>Kezdés dátuma és ideje</label>
                                            <div className="input-field">
                                                <input type="datetime-local" required value={singleData.kezdes} onChange={e => setSingleData({...singleData, kezdes: e.target.value})} />
                                            </div>
                                        </div>
                                        <div className="form-row">
                                            <div className="input-group-wrapper number-col">
                                                <label>Időtartam (perc)</label>
                                                <div className="input-field">
                                                    <input type="number" min="30" max="240" required value={singleData.idotartam} onChange={e => setSingleData({...singleData, idotartam: e.target.value})} />
                                                </div>
                                            </div>
                                            <div className="input-group-wrapper number-col">
                                                <label>Ár (Ft)</label>
                                                <div className="input-field">
                                                    <input type="number" required value={singleData.ar} onChange={e => setSingleData({...singleData, ar: e.target.value})} />
                                                </div>
                                            </div>
                                        </div>
                                        <div className="input-group-wrapper">
                                            <label>Megjegyzés</label>
                                            <div className="input-field">
                                                <input type="text" placeholder="Pl: Találkozó a rutinpályán" value={singleData.megjegyzes} onChange={e => setSingleData({...singleData, megjegyzes: e.target.value})} />
                                            </div>
                                        </div>
                                    </>
                                )}

                                {mode === 'multi' && (
                                    <div className="multi-appointments-container">
                                        {multiData.map((row, index) => (
                                            <div key={index} className="multi-card">
                                                <h4>{index + 1}. Időpont</h4>
                                                
                                                {multiData.length > 1 && (
                                                    <button type="button" className="btn-remove-row" onClick={() => removeAppointmentRow(index)}>
                                                        Törlés
                                                    </button>
                                                )}

                                                <div className="form-row">
                                                    <div className="input-group-wrapper datetime-col">
                                                        <label>Kezdés</label>
                                                        <div className="input-field">
                                                            <input type="datetime-local" required value={row.kezdes} onChange={e => handleMultiChange(index, 'kezdes', e.target.value)} />
                                                        </div>
                                                    </div>
                                                    <div className="input-group-wrapper number-col">
                                                        <label>Perc</label>
                                                        <div className="input-field">
                                                            <input type="number" min="30" max="240" title="Perc" required value={row.idotartam} onChange={e => handleMultiChange(index, 'idotartam', e.target.value)} />
                                                        </div>
                                                    </div>
                                                    <div className="input-group-wrapper number-col">
                                                        <label>Ár (Ft)</label>
                                                        <div className="input-field">
                                                            <input type="number" title="Ár" required value={row.ar} onChange={e => handleMultiChange(index, 'ar', e.target.value)} />
                                                        </div>
                                                    </div>
                                                </div>
                                                <div className="input-group-wrapper" style={{ marginBottom: '0' }}>
                                                    <div className="input-field">
                                                        <input type="text" placeholder="Megjegyzés (opcionális)" value={row.megjegyzes} onChange={e => handleMultiChange(index, 'megjegyzes', e.target.value)} />
                                                    </div>
                                                </div>
                                            </div>
                                        ))}
                                        
                                        <button type="button" className="btn-add-row" onClick={addAppointmentRow}>
                                            + Új időpont hozzáadása
                                        </button>
                                    </div>
                                )}

                                {error && <div className="regist-error">{error}</div>}
                                {successMsg && <div className="regist-success">{successMsg}</div>}

                                <div style={{ display: 'flex', gap: '10px', marginTop: '15px' }}>
                                    <button type="button" className="btn-reg" style={{ backgroundColor: '#a0aec0', flex: 1 }} onClick={() => setMode(null)}>
                                        Módváltás
                                    </button>
                                    <button type="submit" className="btn-reg" style={{ flex: 2 }}>
                                        Mentés
                                    </button>
                                </div>
                            </form>
                        )}
                    </div>
                </div>
            </div>
        </div>
    );
}

export default CreateAppointment;