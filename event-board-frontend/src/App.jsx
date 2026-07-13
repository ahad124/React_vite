import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import EventList from './components/EventList';
import EventDetail from './components/EventDetail';
import './App.css';

function App() {
  return (
    <Router>
      <div className="d-flex flex-column min-vh-100 bg-light-gray">
        {/* Navigation Bar */}
        <nav className="navbar navbar-expand-lg navbar-dark bg-dark-glass sticky-top shadow-sm py-3">
          <div className="container">
            <Link to="/" className="navbar-brand d-flex align-items-center gap-2 fw-bold text-gradient-nav">
              <svg xmlns="http://www.w3.org/2000/svg" width="24" height="24" fill="currentColor" className="bi bi-grid-3x3-gap-fill text-primary" viewBox="0 0 16 16">
                <path d="M1 2a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1v2a1 1 0 0 1-1 1H2a1 1 0 0 1-1-1zm5 0a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1v2a1 1 0 0 1-1 1H7a1 1 0 0 1-1-1zm5 0a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1v2a1 1 0 0 1-1 1h-2a1 1 0 0 1-1-1zM1 7a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1v2a1 1 0 0 1-1 1H2a1 1 0 0 1-1-1zm5 0a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1v2a1 1 0 0 1-1 1H7a1 1 0 0 1-1-1zm5 0a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1v2a1 1 0 0 1-1 1h-2a1 1 0 0 1-1-1zM1 12a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1v2a1 1 0 0 1-1 1H2a1 1 0 0 1-1-1zm5 0a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1v2a1 1 0 0 1-1 1H7a1 1 0 0 1-1-1zm5 0a1 1 0 0 1 1-1h2a1 1 0 0 1 1 1v2a1 1 0 0 1-1 1h-2a1 1 0 0 1-1-1z"/>
              </svg>
              <span>EventBoard</span>
            </Link>
            <button 
              className="navbar-toggler" 
              type="button" 
              data-bs-toggle="collapse" 
              data-bs-target="#navbarNav" 
              aria-controls="navbarNav" 
              aria-expanded="false" 
              aria-label="Toggle navigation"
            >
              <span className="navbar-toggler-icon"></span>
            </button>
            <div className="collapse navbar-collapse justify-content-end" id="navbarNav">
              <ul className="navbar-nav gap-2">
                <li className="nav-item">
                  <Link to="/" className="nav-link px-3 active rounded-pill">Home</Link>
                </li>
                <li className="nav-item">
                  <a href="#about" className="nav-link px-3 rounded-pill text-muted pointer-disabled" onClick={(e) => e.preventDefault()}>About</a>
                </li>
                <li className="nav-item">
                  <button className="btn btn-primary rounded-pill px-4 ms-2 shadow-xs fw-semibold">
                    Sign In
                  </button>
                </li>
              </ul>
            </div>
          </div>
        </nav>

        {/* Main Content Area */}
        <main className="flex-grow-1">
          <Routes>
            <Route path="/" element={<EventList />} />
            <Route path="/event/:id" element={<EventDetail />} />
          </Routes>
        </main>

        {/* Footer */}
        <footer className="bg-dark text-white-50 py-4 mt-auto border-top border-secondary">
          <div className="container text-center">
            <p className="mb-0 small">&copy; {new Date().getFullYear()} EventBoard Frontend. All rights reserved.</p>
          </div>
        </footer>
      </div>
    </Router>
  );
}

export default App;
