import { useState } from 'react';
import './Regist.css';
import { Link, useNavigate } from 'react-router-dom';
import axios from 'axios'

function Regist() {

    const navigate = useNavigate();
    const [name, setName] = useState("");
    const [email, setEmail] = useState("");
    const [phone, setPhone] = useState("");
    const [address, setAddress] = useState("");
    const [password, setPassword] = useState("");
    const [r_password, setR_password] = useState("");
    const [birthday, setBirthday] = useState("");
    const [accept, setAccept] = useState(false);
    const [errorList, setErrorList] = useState([]);
    const [successMsg, setSuccessMsg] = useState("");

    const errors = [
        { errorThis: false, text: "Nem adtál meg nevet!" },
        { errorThis: false, text: "Nem teljes egész nevet adtál meg nagy kezdőbetükkel!" },
        { errorThis: false, text: "Nem adtál meg e-mail címet!" },
        { errorThis: false, text: "Nem megfelelő az e-mail cím formátuma!" },
        { errorThis: false, text: "A jelszó túl rövid!" },
        { errorThis: false, text: "A jelszóban nem szerepel nagybetű!" },
        { errorThis: false, text: "A jelszóban nem szerepel szám!" },
        { errorThis: false, text: "A jelszóban nem szerepel különleges karakter!" },
        { errorThis: false, text: "Olyan karaktert is tartalmaz, amelyet nem szabad!" },
        { errorThis: false, text: "A két jelszó nem egyezik meg!" },
        { errorThis: false, text: "Nem múlt el még 14 éves!" },
        { errorThis: false, text: "Nem fogadtad el a felhasználói feltételeket!" },
        { errorThis: false, text: "Nem adtál meg telefonszámot!" },
        { errorThis: false, text: "A telefonszám formátuma nem megfelelő! (pl: +36301234567)" },
        { errorThis: false, text: "Nem adtál meg lakcímet!" },
        { errorThis: false, text: "A lakcím túl rövid!" }
    ];

    const handleRegist = async (e) => {
        e.preventDefault();
        setErrorList([]);
        setSuccessMsg("");

        let tempErrors = JSON.parse(JSON.stringify(errors));
        const today = new Date();

        if (!name) tempErrors[0].errorThis = true;
        else if (!/^[A-ZÁÉÖÜÓŐÚŰÍ][a-zöüóőúéáűí]+ [A-ZÁÉÖÜÓŐÚŰÍ][a-zöüóőúéáűí]+/.test(name)) {
            tempErrors[1].errorThis = true;
        }

        if (!email) tempErrors[2].errorThis = true;
        else if (!/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email)) tempErrors[3].errorThis = true;

        if (password.length < 8) tempErrors[4].errorThis = true;
        if (!/[A-Z]/.test(password)) tempErrors[5].errorThis = true;
        if (!/[0-9]/.test(password)) tempErrors[6].errorThis = true;
        if (!/[*!$,%?;+@#<>\-_=\/:\\]/.test(password)) tempErrors[7].errorThis = true;

        if (password !== r_password) tempErrors[9].errorThis = true;

        if (!birthday) {
            tempErrors[10].errorThis = true;
        } else {
            const birthDate = new Date(birthday);
            let age = today.getFullYear() - birthDate.getFullYear();
            const m = today.getMonth() - birthDate.getMonth();
            if (m < 0 || (m === 0 && today.getDate() < birthDate.getDate())) {
                age--;
            }
            if (age < 14) tempErrors[10].errorThis = true;
        }

        if (!accept) tempErrors[11].errorThis = true;

        if (!phone) {
            tempErrors[12].errorThis = true;
        } else {
            const phoneRegex = /^(?:\+36|06)(?:20|30|31|70)\d{7}$/;
            if (!phoneRegex.test(phone.replace(/\s/g, ""))) {
                tempErrors[13].errorThis = true;
            }
        }

        if (!address) {
            tempErrors[14].errorThis = true;
        } else if (address.length < 10) {
            tempErrors[15].errorThis = true;
        }

        setErrorList(tempErrors);

        const hasError = tempErrors.some(err => err.errorThis);
        if (hasError) return;

        try {
            const response = await axios.post('https://localhost:7200/api/Auth/register', {
                Nev: name,
                Email: email,
                Jelszo: password,
                Telefonszam: phone,
                Cim: address
            });

            if (response.status === 201 || response.status === 200) {
                setSuccessMsg("Sikeres regisztráció! Pillanatokon belül átirányítunk...");

                setTimeout(() => {
                    navigate('/');
                }, 3000);
            }
        } catch (err) {
            const errorText = err.response?.data || "Szerver nem elérhető";
            setSuccessMsg("");
            alert("Hiba: " + errorText);
        };
    }

    return (
        <>
            <div className="main-wrapper">
                <div className="split-container regist-card">
                    <div className="left-side">
                        <div className="overlay"></div>
                        <div className="left-content fade-in">
                            <h2 className="logo-text">Jogsifoglaló</h2>
                            <div className="hero-text">
                                <h1>Kezdd el a jogsid még ma!</h1>
                                <p>Foglalj időpontot, kövesd a haladásod és szerezd meg a jogosítványod gyorsabban!</p>
                            </div>
                        </div>
                    </div>

                    <div className="right-side slide-up">
                        <div className="form-box">
                            <h2>Fiók létrehozása</h2>
                            <form className="reg-form" onSubmit={handleRegist}>
                                <div className="input-field">
                                    <input type="text" placeholder="Név" required value={name} onChange={e => setName(e.target.value)} />
                                </div>
                                <div className="input-field">
                                    <input type="email" placeholder="Email" required value={email} onChange={e => setEmail(e.target.value)} />
                                </div>
                                <div className="input-field">
                                    <input type="tel" placeholder="Telefonszám (+36...)" maxLength="12" required value={phone} onChange={e => setPhone(e.target.value)} />
                                </div>
                                <div className="input-field">
                                    <input type="text" placeholder="Cím" value={address} onChange={e => setAddress(e.target.value)} />
                                </div>
                                <div className="input-field">
                                    <input type="password" placeholder="Jelszó" value={password} onChange={e => setPassword(e.target.value)} required />
                                </div>
                                <div className="input-field">
                                    <input type="password" placeholder="Jelszó újra" value={r_password} onChange={e => setR_password(e.target.value)} required />
                                </div>
                                <div className="input-field">
                                    <input type="date" placeholder="éééé. hh. nn." value={birthday} onChange={e => setBirthday(e.target.value)} />
                                </div>

                                <div className="checkbox-row">
                                    <input type="checkbox" id="accept" checked={accept} onChange={e => setAccept(e.target.checked)} />
                                    <label htmlFor="accept">Elfogadom a feltételeket</label>
                                </div>

                                {errorList.filter(e => e.errorThis).map((err, i) => (
                                    <div key={i} className="regist-error" style={{ color: 'red', fontSize: '12px' }}>{err.text}</div>
                                ))}

                                {successMsg && <div className="regist-success">{successMsg}</div>}

                                <button type="submit" className="btn-reg">Regisztráció</button>
                                <Link to='/' className="btn-login-link">Bejelentkezés</Link>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
        </>
    )
}

export default Regist