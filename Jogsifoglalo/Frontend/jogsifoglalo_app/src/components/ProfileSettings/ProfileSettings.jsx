import { useState, useEffect } from 'react';
import axios from 'axios';
import Swal from 'sweetalert2';
import Sidebar from '../Sidebar/Sidebar';
import { useNavigate } from 'react-router-dom'
import { useAuth } from "../../Context/AuthContext";
import './ProfileSettings.css';

function ProfileSettings() {
    const { user, token, setUser } = useAuth();
    const navigate = useNavigate();
    
    const [formData, setFormData] = useState({
        nev: '', email: '', telefonszam: '', cim: ''
    });

    useEffect(() => {
        if (user) {
            setFormData({
                nev: user.nev || '',
                email: user.email || '',
                telefonszam: user.telefonszam || '',
                cim: user.cim || ''
            });
        }
    }, [user]);

    const handleSave = async (e) => {
        e.preventDefault();
        try {
            await axios.put('https://localhost:7200/api/Users/update-me', formData, {
                headers: { Authorization: `Bearer ${token}` }
            });

            setUser({ ...user, ...formData });

            Swal.fire({
                title: 'Sikeres mentés!',
                text: 'A profiladataidat frissítettük.',
                icon: 'success',
                confirmButtonColor: '#1877f2'
            });

        } catch (err) {
            Swal.fire('Hiba!', err.response?.data || 'A mentés nem sikerült.', 'error');
        }
    };

    const handleDelete = async () => {
        const result = await Swal.fire({
            title: 'Biztosan törölni akarod a profilod?',
            text: 'Ez a művelet nem vonható vissza!',
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#dc3545',
            cancelButtonColor: '#6c757d', 
            confirmButtonText: 'Igen, törlöm!',
            cancelButtonText: 'Mégsem'
        });

        if (result.isConfirmed) {
            try {
                await axios.delete('https://localhost:7200/api/Users/delete-me', {
                    headers: { Authorization: `Bearer ${token}` }
                });

                await Swal.fire({
                    title: 'Törölve!',
                    text: 'A profilod sikeresen törlésre került.',
                    icon: 'success',
                    confirmButtonColor: '#1877f2'
                });

                setUser(null); 
                localStorage.removeItem('token');
                setTimeout(() => {
                    navigate('/');
                }, 1000);
            } catch (err) {
                Swal.fire('Hiba!', err.response?.data || 'A törlés nem sikerült.', 'error');
            }
        }
    };

    if (!user) return <div className="page-container"><Sidebar /></div>;

    return (
        <div className="page-container">
            <Sidebar />
            <div className="content-area fade-in">
                <div className="header-section">
                    <h1>Beállítások </h1>
                    <p>Módosítsd a profiladataidat az alábbi űrlapon.</p>
                </div>

                <div className="dash-card profile-card">
                    <form className="profile-form" onSubmit={handleSave}>
                        <div className="input-group">
                            <label>Teljes Név</label>
                            <input 
                                type="text" required
                                value={formData.nev} 
                                onChange={(e) => setFormData({...formData, nev: e.target.value})} 
                            />
                        </div>
                        <div className="input-group">
                            <label>Email Cím</label>
                            <input 
                                type="email" required
                                value={formData.email} 
                                onChange={(e) => setFormData({...formData, email: e.target.value})} 
                            />
                        </div>
                        <div className="input-group">
                            <label>Telefonszám</label>
                            <input 
                                type="text" required
                                value={formData.telefonszam} 
                                onChange={(e) => setFormData({...formData, telefonszam: e.target.value})} 
                            />
                        </div>
                        <div className="input-group">
                            <label>Lakcím</label>
                            <input 
                                type="text" required
                                value={formData.cim} 
                                onChange={(e) => setFormData({...formData, cim: e.target.value})} 
                            />
                        </div>

                        <button type="submit" className="btn-save-profile">Módosítások mentése</button>
                    </form>

                    <div className="danger-zone">
                      
                        <button type="button" className="btn-delete-profile" onClick={handleDelete}>
                            Fiók törlése
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default ProfileSettings;