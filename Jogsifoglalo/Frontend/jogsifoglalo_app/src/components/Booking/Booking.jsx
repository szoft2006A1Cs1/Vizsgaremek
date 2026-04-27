import { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import Sidebar from '../Sidebar/Sidebar';
import './Booking.css';

function Booking() {
    const navigate = useNavigate();
    const [token] = useState(localStorage.getItem('token'));
    
    const [currentWeekStart, setCurrentWeekStart] = useState(getMonday(new Date()));
    
    const [allAppointments, setAllAppointments] = useState([]); 
    const [appointments, setAppointments] = useState([]); 
    
    const [categories, setCategories] = useState([]);
    const [instructors, setInstructors] = useState([]);
    
    const [selectedCategory, setSelectedCategory] = useState("");
    const [selectedInstructor, setSelectedInstructor] = useState("");
    const [error, setError] = useState("");

    const weekDays = [0, 1, 2, 3, 4, 5, 6]; 

    function getMonday(d) {
        let date = new Date(d);
        var day = date.getDay(), diff = date.getDate() - day + (day === 0 ? -6 : 1);
        return new Date(date.setDate(diff));
    }

    useEffect(() => {
        if (!token) {
            navigate('/');
            return;
        }
        fetchFilters();
        fetchAllAppointments();
    }, [token, navigate]);

    const fetchFilters = async () => {
        try {
            const catRes = await axios.get('https://localhost:7200/api/Categories');
            setCategories(catRes.data);

            const instRes = await axios.get('https://localhost:7200/api/Instructors');
            setInstructors(instRes.data);
        } catch (err) {
            console.error("Hiba a szűrők betöltésekor", err);
        }
    };

    const fetchAllAppointments = async () => {
        setError("");
        try {
            const response = await axios.get('https://localhost:7200/api/Appointments/available', {
                headers: { Authorization: `Bearer ${token}` }
            });
            
            const now = new Date();
            const futureAppointments = response.data.filter(app => new Date(app.kezdes_Dt) > now);
            
            setAllAppointments(futureAppointments);
        } catch (err) {
            if (err.response?.status === 404) {
                setAllAppointments([]);
            } else {
                setError("Hiba történt az időpontok betöltésekor.");
            }
        }
    };

  
    useEffect(() => {
        let filteredData = [...allAppointments];

        if (selectedCategory) {
            const selectedCatObj = categories.find(c => c.kategoria_Kod === selectedCategory);
            if (selectedCatObj) {
                filteredData = filteredData.filter(a => a.kategoria_Nev === selectedCatObj.kategoria_Nev);
            }
        }

        if (selectedInstructor) {
            const instObj = instructors.find(i => i.oktato_Id.toString() === selectedInstructor.toString());
            if (instObj) {
                filteredData = filteredData.filter(a => a.oktato_Nev === instObj.nev);
            }
        }

        setAppointments(filteredData);
    }, [allAppointments, selectedCategory, selectedInstructor, categories, instructors]);

    const getFilteredInstructors = () => {
        if (!selectedCategory) return instructors; 

        const selectedCatObj = categories.find(c => c.kategoria_Kod === selectedCategory);
        if (!selectedCatObj) return instructors;

        const validInstructorNames = [...new Set(
            allAppointments
                .filter(a => a.kategoria_Nev === selectedCatObj.kategoria_Nev)
                .map(a => a.oktato_Nev)
        )];

        return instructors.filter(inst => validInstructorNames.includes(inst.nev));
    };

    const displayedInstructors = getFilteredInstructors();

    const handlePrevWeek = () => {
        const newDate = new Date(currentWeekStart);
        newDate.setDate(newDate.getDate() - 7);
        setCurrentWeekStart(newDate);
    };

    const handleNextWeek = () => {
        const newDate = new Date(currentWeekStart);
        newDate.setDate(newDate.getDate() + 7);
        setCurrentWeekStart(newDate);
    };

    const handleBookingClick = (appointment) => {
        navigate('/payment', { state: { appointment } });
    };

    const getAppointmentsForDay = (dayOffset) => {
        const targetDate = new Date(currentWeekStart);
        targetDate.setDate(targetDate.getDate() + dayOffset);
        const dateString = targetDate.toLocaleDateString('hu-HU'); 

        const dailyApps = appointments.filter(a => {
            const appDateString = new Date(a.kezdes_Dt).toLocaleDateString('hu-HU');
            return appDateString === dateString;
        });

        return dailyApps.sort((a, b) => new Date(a.kezdes_Dt) - new Date(b.kezdes_Dt));
    };

    const isCurrentWeek = currentWeekStart.getTime() <= getMonday(new Date()).getTime();

    return (
        <div className="page-container">
            <Sidebar />
            
            <div className="content-area fade-in">
                <div className="booking-header">
                    <div>
                        <h1>Foglalható időpontok</h1>
                        <p>Keresd meg a számodra megfelelő időpontot és oktatót.</p>
                    </div>
                </div>

                <div className="filter-section dash-card">
                    <div className="filter-group">
                        <label>Kategória</label>
                        <select 
                            value={selectedCategory} 
                            onChange={(e) => {
                                setSelectedCategory(e.target.value);
                                setSelectedInstructor("");
                            }}
                        >
                            <option value="">Összes kategória</option>
                            {categories.map(cat => (
                                <option key={cat.kategoria_Id} value={cat.kategoria_Kod}>
                                    {cat.kategoria_Kod} - {cat.kategoria_Nev}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div className="filter-group">
                        <label>Oktató</label>
                        <select value={selectedInstructor} onChange={(e) => setSelectedInstructor(e.target.value)}>
                            <option value="">Bárki</option>
                            {displayedInstructors.map(inst => (
                                <option key={inst.oktato_Id} value={inst.oktato_Id}>
                                    {inst.nev}
                                </option>
                            ))}
                        </select>
                    </div>

                    <div className="filter-action">
                        <button className="btn-filter" onClick={fetchAllAppointments}>
                            Naptár frissítése
                        </button>
                    </div>
                </div>

                {error && <div className="regist-error">{error}</div>}

                <div className="calendar-section dash-card">
                    <div className="calendar-controls">
                        <h3>
                            {currentWeekStart.toLocaleDateString('hu-HU', { year: 'numeric', month: 'long', day: 'numeric' })} 
                            {' - '} 
                            {new Date(currentWeekStart.getTime() + 6 * 24 * 60 * 60 * 1000).toLocaleDateString('hu-HU', { month: 'long', day: 'numeric' })}
                        </h3>
                        <div className="week-nav-btns">
                            <button 
                                onClick={handlePrevWeek} 
                                disabled={isCurrentWeek}
                                style={{ opacity: isCurrentWeek ? 0.3 : 1, cursor: isCurrentWeek ? 'not-allowed' : 'pointer' }}
                            >
                                &lt;
                            </button>
                            <button onClick={handleNextWeek}>&gt;</button>
                        </div>
                    </div>

                    <div className="flexible-calendar-grid">
                        {weekDays.map(offset => {
                            const dayDate = new Date(currentWeekStart);
                            dayDate.setDate(dayDate.getDate() + offset);
                            const dailyAppointments = getAppointmentsForDay(offset);

                            return (
                                <div key={offset} className="flex-day-column">
                                    <div className="day-header">
                                        <div className="day-name">{['HÉTFŐ', 'KEDD', 'SZERDA', 'CSÜTÖRTÖK', 'PÉNTEK', 'SZOMBAT', 'VASÁRNAP'][offset]}</div>
                                        <div className="day-date">{dayDate.toLocaleDateString('hu-HU', { month: 'short', day: 'numeric' })}</div>
                                    </div>
                                    <div className="day-slots">
                                        {dailyAppointments.length > 0 ? (
                                            dailyAppointments.map(app => (
                                                <div key={app.idopont_Id} className="flexible-slot" onClick={() => handleBookingClick(app)}>
                                                    <div className="slot-time">
                                                        {new Date(app.kezdes_Dt).toLocaleTimeString('hu-HU', { hour: '2-digit', minute: '2-digit' })}
                                                    </div>
                                                    <div className="slot-status">SZABAD</div>
                                                    <div className="slot-inst">{app.oktato_Nev}</div>
                                                </div>
                                            ))
                                        ) : (
                                            <div className="slot-empty">
                                                <span>Nincs elérhető óra</span>
                                            </div>
                                        )}
                                    </div>
                                </div>
                            );
                        })}
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Booking;