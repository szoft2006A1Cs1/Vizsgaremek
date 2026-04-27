import { useState } from 'react';
import './ForgotPassword.css';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import Swal from 'sweetalert2';
import Sidebar from '../Sidebar/Sidebar';
import { useAuth } from '../../Context/AuthContext';

function ForgotPassword() {
    const navigate = useNavigate();
    const { token, logout, user } = useAuth();
    
    const [oldPassword, setOldPassword] = useState("");
    const [newPassword, setNewPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [error, setError] = useState("");

    const resetPassword = async (e) => {
        e.preventDefault();
        setError("");

        if (newPassword !== confirmPassword) {
            setError("A két új jelszó nem egyezik meg!");
            return;
        }

        if (oldPassword === newPassword) {
            setError("Az új jelszó nem lehet ugyanaz, mint a régi!");
            return;
        }

        try {
            const response = await axios.put('https://localhost:7200/api/Users/change-password',
                {
                    regiJelszo: oldPassword,
                    ujJelszo: newPassword
                },
                {
                    headers: { Authorization: `Bearer ${token}` }
                });

            if (response.status === 200) {
                await Swal.fire({
                    title: 'Sikeres jelszócsere!',
                    text: 'A biztonság érdekében kérjük, jelentkezz be újra az új jelszavaddal.',
                    icon: 'success',
                    confirmButtonColor: '#1877f2'
                });
                
                logout(); 
                navigate('/');
            }
        } catch (err) {
            setError(err.response?.data || "Hiba történt a jelszó módosítása közben.");
        }
    };

    return (
        <div className="page-container">
            <Sidebar />
            <div className="content-area fade-in">
                <div className="header-section">
                    <h1>Biztonsági beállítások</h1>
                    <p>Itt módosíthatod a fiókodhoz tartozó jelszót.</p>
                </div>

                <div className="dash-card profile-card" style={{ maxWidth: '500px', margin: '0 auto' }}>
                    <form className="profile-form" onSubmit={resetPassword}>
                        
                        <div className="input-group">
                            <label>Jelenlegi jelszó</label>
                            <input 
                                type="password" 
                                value={oldPassword} 
                                onChange={e => setOldPassword(e.target.value)} 
                                required 
                            />
                        </div>

                        <div className="input-group">
                            <label>Új jelszó</label>
                            <input 
                                type="password" 
                                value={newPassword} 
                                onChange={e => setNewPassword(e.target.value)} 
                                required 
                            />
                        </div>

                        <div className="input-group">
                            <label>Új jelszó újra</label>
                            <input 
                                type="password" 
                                value={confirmPassword} 
                                onChange={e => setConfirmPassword(e.target.value)} 
                                required 
                            />
                        </div>
                        
                        {error && <div className="regist-error" style={{marginBottom: '15px', color: 'red'}}>{error}</div>}

                        <button type="submit" className="btn-save-profile">Jelszó frissítése</button>
                    </form>
                </div>
            </div>
        </div>
    );
}

export default ForgotPassword;