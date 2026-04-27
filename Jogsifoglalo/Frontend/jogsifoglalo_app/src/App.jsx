import './App.css';
import { BrowserRouter, Routes, Route } from 'react-router-dom';
import { AuthProvider } from './Context/AuthContext';
import ProtectedRoute from './Context/ProtectedRoute';
import Login from './components/Login/Login';
import Regist from './components/Regist/Regist';
import ForgotPassword from './components/ForgotPassword/ForgotPassword';
import Dashboard from './components/Dashboard/Dashboard';
import Booking from './components/Booking/Booking';
import CreateAppointment from './components/CreateAppointment/CreateAppointment';
import AdminDashboard from './components/AdminDashboard/AdminDashboard';
import Payment from './components/Payment/Payment'; 
import ProfileSettings from './components/ProfileSettings/ProfileSettings';
import Progress from './components/Progress/Progress';

function App() {
  return (
    <BrowserRouter>
      <AuthProvider>
        <Routes>
          <Route path='/' element={<Login />} />
          <Route path='/regist' element={<Regist />} />
          <Route path='/dashboard' element={
            <ProtectedRoute><Dashboard /></ProtectedRoute>
          } />
          <Route path='/profile' element={
            <ProtectedRoute><ProfileSettings /></ProtectedRoute>
          } />
          <Route path='/forgot-password' element={
            <ProtectedRoute><ForgotPassword /></ProtectedRoute>
          } />
          <Route path='/progress' element={
            <ProtectedRoute><Progress /></ProtectedRoute>
          } />
          <Route path='/booking' element={
            <ProtectedRoute requiredRole="tanulo"><Booking /></ProtectedRoute>
          } />          
          <Route path='/payment' element={
            <ProtectedRoute requiredRole="tanulo"><Payment /></ProtectedRoute>
          } />
          <Route path='/create-appointment' element={
            <ProtectedRoute requiredRole="oktato"><CreateAppointment /></ProtectedRoute>
          } />
          <Route path='/admin' element={
            <ProtectedRoute requiredRole="admin"><AdminDashboard /></ProtectedRoute>
          } />          
        </Routes>
      </AuthProvider>
    </BrowserRouter>
  );
}

export default App;