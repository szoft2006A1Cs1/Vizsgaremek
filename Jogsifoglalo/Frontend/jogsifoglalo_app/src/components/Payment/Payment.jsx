import { useState, useEffect } from 'react';
import { useNavigate, useLocation } from 'react-router-dom';
import axios from 'axios';
import Sidebar from '../Sidebar/Sidebar';
import './Payment.css';

function Payment() {
    const navigate = useNavigate();
    const location = useLocation();
    const [token] = useState(localStorage.getItem('token'));
    
    const [appointmentDetails, setAppointmentDetails] = useState(null);
    const [paymentMethod, setPaymentMethod] = useState("");
    
    const [cardName, setCardName] = useState("");
    const [cardNumber, setCardNumber] = useState("");
    const [expiry, setExpiry] = useState("");
    const [cvv, setCvv] = useState("");
    const [error, setError] = useState("");
    const [isProcessing, setIsProcessing] = useState(false);

    useEffect(() => {
        if (!token) {
            navigate('/');
            return;
        }

        if (location.state && location.state.appointment) {
            setAppointmentDetails(location.state.appointment);
        } else {
            navigate('/booking'); 
        }
    }, [token, navigate, location]);

    const handlePayment = async (e) => {
        if (e) e.preventDefault();
        setError("");
        setIsProcessing(true);

        try {
            await axios.post('https://localhost:7200/api/Payments', {
                Felhasznalo_Id: 0, 
                Idopont_Id: appointmentDetails.idopont_Id,
                Osszeg: appointmentDetails.ar
            }, {
                headers: { Authorization: `Bearer ${token}` }
            });

            setTimeout(() => {
                navigate('/dashboard');
            }, 1500);

        } catch (err) {
            setError(err.response?.data || "Hiba történt a fizetés feldolgozása közben.");
            setIsProcessing(false);
        }
    };

    if (!appointmentDetails) return null;

    const bookingFee = 490;
    const totalAmount = appointmentDetails.ar + bookingFee;
    const dateObj = new Date(appointmentDetails.kezdes_Dt);
    const formattedDate = dateObj.toLocaleDateString('hu-HU', { year: 'numeric', month: 'long', day: 'numeric' });
    const formattedTime = dateObj.toLocaleTimeString('hu-HU', { hour: '2-digit', minute: '2-digit' });

    return (
        <div className="page-container">
            <Sidebar />
            
            <div className="content-area fade-in">
                <div className="header-section">
                    <h1>Fizetés és Foglalás</h1>
                    <p>Válassz fizetési módot a foglalás véglegesítéséhez.</p>
                </div>

                <div className="payment-grid">
                    <div className="payment-form-section dash-card">
                        <h3>Fizetési mód kiválasztása</h3>
                        
                        <div className="payment-methods">
                            <div 
                                className={`method-card ${paymentMethod === 'onsite' ? 'active' : ''}`} 
                                onClick={() => setPaymentMethod('onsite')}
                            >
                                <div className="method-icon">💵</div>
                                <div className="method-details">
                                    <strong>Fizetés a helyszínen</strong>
                                    <span>Készpénz vagy kártya az oktatónál a találkozáskor</span>
                                </div>
                            </div>

                            <div 
                                className={`method-card ${paymentMethod === 'card' ? 'active' : ''}`} 
                                onClick={() => setPaymentMethod('card')}
                            >
                                <div className="method-icon">💳</div>
                                <div className="method-details">
                                    <strong>Online bankkártyás fizetés</strong>
                                    <span>Azonnali és biztonságos fizetés most</span>
                                </div>
                            </div>
                        </div>

                        {paymentMethod === 'onsite' && (
                            <div className="onsite-form slide-up">
                                <p className="onsite-info">A vezetési óra díját és a foglalási díjat a helyszínen, az oktatónál tudod majd rendezni. A foglalásod azonnal rögzítésre kerül.</p>
                                <button onClick={handlePayment} className="btn-pay-submit" disabled={isProcessing}>
                                    {isProcessing ? 'Feldolgozás...' : 'Foglalás véglegesítése →'}
                                </button>
                            </div>
                        )}

                        {paymentMethod === 'card' && (
                            <form onSubmit={handlePayment} className="card-form slide-up">
                                <div className="input-group">
                                    <label>Kártyabirtokos neve</label>
                                    <input 
                                        type="text" 
                                        placeholder="Minta János" 
                                        required 
                                        value={cardName}
                                        onChange={e => setCardName(e.target.value)}
                                    />
                                </div>

                                <div className="input-group">
                                    <label>Kártyaszám</label>
                                    <input 
                                        type="text" 
                                        placeholder="0000 0000 0000 0000" 
                                        maxLength="19" 
                                        required 
                                        value={cardNumber}
                                        onChange={e => setCardNumber(e.target.value)}
                                    />
                                </div>

                                <div className="card-row">
                                    <div className="input-group">
                                        <label>Lejárati dátum</label>
                                        <input 
                                            type="text" 
                                            placeholder="MM / YY" 
                                            maxLength="7" 
                                            required 
                                            value={expiry}
                                            onChange={e => setExpiry(e.target.value)}
                                        />
                                    </div>
                                    <div className="input-group">
                                        <label>CVV</label>
                                        <input 
                                            type="text" 
                                            placeholder="123" 
                                            maxLength="3" 
                                            required 
                                            value={cvv}
                                            onChange={e => setCvv(e.target.value)}
                                        />
                                    </div>
                                </div>

                                {error && <div className="regist-error" style={{marginTop: '15px'}}>{error}</div>}

                                <button 
                                    type="submit" 
                                    className="btn-pay-submit" 
                                    disabled={isProcessing}
                                >
                                    {isProcessing ? 'Feldolgozás...' : 'Fizetés és Foglalás →'}
                                </button>
                                
                                <p className="secure-text">🔒 Biztonságos tranzakció a Jogsifoglaló rendszerén keresztül</p>
                            </form>
                        )}
                    </div>

                    <div className="payment-summary-section dash-card">
                        <h3>Foglalási összegzés</h3>
                        
                        <div className="summary-item main-item">
                            <div className="cat-badge">{appointmentDetails.kategoria_Kod || 'B'} KATEGÓRIA</div>
                            <h4>Vezetési óra - {appointmentDetails.kategoria_Nev || 'Forgalmi felkészítés'}</h4>
                            <span className="duration-text">⏱ {appointmentDetails.idotartam} perces gyakorlati óra</span>
                        </div>

                        <div className="summary-details">
                            <div className="detail-row">
                                <div className="icon-box">📅</div>
                                <div>
                                    <span className="label">IDŐPONT</span>
                                    <strong>{formattedDate}</strong>
                                    <span>{formattedTime}</span>
                                </div>
                            </div>

                            <div className="detail-row">
                                <div className="icon-box">👤</div>
                                <div>
                                    <span className="label">OKTATÓ</span>
                                    <strong>{appointmentDetails.oktato_Nev}</strong>
                                    <span>Minősített szakoktató</span>
                                </div>
                            </div>
                        </div>

                        <div className="price-breakdown">
                            <div className="price-row">
                                <span>Alapdíj (1 óra)</span>
                                <span>{appointmentDetails.ar.toLocaleString('hu-HU')} Ft</span>
                            </div>
                            <div className="price-row">
                                <span>Foglalási szolgáltatási díj</span>
                                <span>{bookingFee.toLocaleString('hu-HU')} Ft</span>
                            </div>
                        </div>

                        <div className="total-price-section">
                            <span>FIZETENDŐ</span>
                            <div className="total-amount">
                                <strong>{totalAmount.toLocaleString('hu-HU')} Ft</strong>
                                <small>ÁFÁVAL EGYÜTT</small>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Payment;