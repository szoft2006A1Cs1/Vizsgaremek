import { useState } from 'react';
import './Login.css'
import { Link, useNavigate } from 'react-router-dom'
import axios from 'axios';
import { useAuth } from '../../Context/AuthContext';

function Login() {
    const navigate = useNavigate();
    const { login } = useAuth();
    
    const [email, setEmail] = useState("");
    const [password, setPassword] = useState("");
    const [error, setError] = useState("");
    const [successMsg, setSuccessMsg] = useState("");

    const handleLogin = async (e) => {
        e.preventDefault();
        setError(""); 

        try {
            const response = await axios.post('https://localhost:7200/api/Auth/login', {
                Email: email,
                Jelszo: password
            });

            if (response.data) {
                login(response.data);
                
                setSuccessMsg("Sikeres bejelentkezés! Pillanatokon belül átirányítunk...");

                setTimeout(() => {
                    navigate('/dashboard');
                }, 1000);
            }
        }
        catch (err) {
            const msg = err.response?.data || "Hibás email vagy jelszó!";
            setError(msg);
            setSuccessMsg("");
        }
    };

    return (
        <div className="main-wrapper">
            <div className="split-container login-card">
                <div className="left-side">
                    <div className="overlay"></div>
                    <div className="left-content fade-in">
                        <h2 className="logo-text">Jogsifoglaló</h2>
                        <div className="hero-text">
                            <h1>Üdvözlünk újra!</h1>
                            <p>Jelentkezz be, és folytasd ott, ahol legutóbb abbahagytad.</p>
                        </div>
                    </div>
                </div>

                <div className="right-side slide-up">
                    <div className="form-box">
                        <h2>Bejelentkezés</h2>

                        <form className="reg-form" onSubmit={handleLogin}>
                            <div className="input-field">
                                <input 
                                    type="email" 
                                    placeholder="Email cím" 
                                    value={email} 
                                    onChange={e => setEmail(e.target.value)} 
                                    required 
                                />
                            </div>

                            <div className="input-field">
                                <input 
                                    type="password" 
                                    placeholder="Jelszó" 
                                    value={password} 
                                    onChange={e => setPassword(e.target.value)} 
                                    required 
                                />
                            </div>

                            {error && <div className="regist-error">{error}</div>}

                            {successMsg && <div className="login-success">{successMsg}</div>}

                            <button type="submit" className="btn-reg">Bejelentkezés</button>

                            <div className="divider"><span>vagy</span></div>

                            <Link to="/regist" className="btn-secondary">Regisztráció</Link>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Login;