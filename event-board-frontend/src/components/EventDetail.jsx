import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import axios from 'axios';

const EventDetail = () => {
  const { id } = useParams();
  const [event, setEvent] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchEventDetail = async () => {
      try {
        setLoading(true);
        const baseUrl = import.meta.env.VITE_API_BASE_URL || 'http://localhost:5000';
        const response = await axios.get(`${baseUrl}/events/${id}`);
        setEvent(response.data);
        setError(null);
      } catch (err) {
        console.error('Error fetching event details:', err);
        setError('Failed to fetch event details. Please verify the event exists and try again.');
      } finally {
        setLoading(false);
      }
    };

    fetchEventDetail();
  }, [id]);

  if (loading) {
    return (
      <div className="d-flex justify-content-center align-items-center min-vh-50 py-5">
        <div className="spinner-border text-primary role-status-spinner" role="status" style={{ width: '3rem', height: '3rem' }}>
          <span className="visually-hidden">Loading event details...</span>
        </div>
      </div>
    );
  }

  if (error || !event) {
    return (
      <div className="container mt-4">
        <div className="alert alert-danger shadow-sm border-start border-danger border-4 d-flex align-items-center mb-4" role="alert">
          <svg className="bi flex-shrink-0 me-2" width="24" height="24" role="img" aria-label="Danger:" fill="currentColor" viewBox="0 0 16 16">
            <path d="M8.982 1.566a1.13 1.13 0 0 0-1.96 0L.165 13.233c-.457.778.091 1.767.98 1.767h13.713c.889 0 1.438-.99.98-1.767L8.982 1.566zM8 5c.535 0 .954.462.9.995l-.35 3.507a.552.552 0 0 1-1.1 0L7.1 5.995A.905.905 0 0 1 8 5zm.002 6a1 1 0 1 1 0 2 1 1 0 0 1 0-2z"/>
          </svg>
          <div>
            <strong>Error:</strong> {error || 'Event details not found.'}
          </div>
        </div>
        <Link to="/" className="btn btn-primary rounded-pill px-4">
          Back to Events
        </Link>
      </div>
    );
  }

  return (
    <div className="container py-5">
      <div className="mb-4">
        <Link to="/" className="btn btn-link text-decoration-none d-inline-flex align-items-center ps-0 text-primary fw-semibold">
          <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-arrow-left me-2" viewBox="0 0 16 16">
            <path fillRule="evenodd" d="M15 8a.5.5 0 0 0-.5-.5H2.707l3.147-3.146a.5.5 0 1 0-.708-.708l-4 4a.5.5 0 0 0 0 .708l4 4a.5.5 0 0 0 .708-.708L2.707 8.5H14.5A.5.5 0 0 0 15 8"/>
          </svg>
          Back to Events
        </Link>
      </div>

      <div className="card shadow-lg border-0 overflow-hidden detail-card">
        {/* Banner area with a gradient */}
        <div className="bg-gradient-primary-dark text-white p-5 position-relative">
          <div className="position-relative z-1">
            <span className="badge rounded-pill bg-white text-primary px-3 py-2 fw-semibold mb-3">
              {event.category || 'General'}
            </span>
            <h1 className="fw-bold display-4 mb-3">{event.title}</h1>
            <div className="d-flex flex-wrap gap-4 text-white-50">
              <span className="d-flex align-items-center">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-calendar-event me-2 text-white" viewBox="0 0 16 16">
                  <path d="M11 6.5a.5.5 0 0 1 .5-.5h1a.5.5 0 0 1 .5.5v1a.5.5 0 0 1-.5.5h-1a.5.5 0 0 1-.5-.5z"/>
                  <path d="M3.5 0a.5.5 0 0 1 .5.5V1h8V.5a.5.5 0 0 1 1 0V1h1a2 2 0 0 1 2 2v11a2 2 0 0 1-2 2H2a2 2 0 0 1-2-2V3a2 2 0 0 1 2-2h1V.5a.5.5 0 0 1 .5-.5M1 4v10a1 1 0 0 0 1 1h12a1 1 0 0 0 1-1V4z"/>
                </svg>
                {new Date(event.date).toLocaleDateString(undefined, {
                  weekday: 'long',
                  month: 'long',
                  day: 'numeric',
                  year: 'numeric',
                  hour: '2-digit',
                  minute: '2-digit'
                })}
              </span>
              <span className="d-flex align-items-center">
                <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-geo-alt-fill me-2 text-white" viewBox="0 0 16 16">
                  <path d="M8 16s6-5.686 6-10A6 6 0 0 0 2 6c0 4.314 6 10 6 10m0-7a3 3 0 1 1 0-6 3 3 0 0 1 0 6"/>
                </svg>
                {event.location}
              </span>
            </div>
          </div>
          <div className="gradient-overlay"></div>
        </div>

        <div className="card-body p-5">
          <div className="row g-5">
            <div className="col-lg-8">
              <h2 className="h4 fw-bold mb-3 border-bottom pb-2">About the Event</h2>
              <p className="lead text-muted fs-5 lh-base mb-4" style={{ whiteSpace: 'pre-line' }}>
                {event.description}
              </p>
              
              {event.agenda && (
                <div className="mt-5">
                  <h3 className="h5 fw-bold mb-3">Agenda</h3>
                  <div className="list-group list-group-flush border-top border-bottom">
                    {event.agenda.map((item, idx) => (
                      <div key={idx} className="list-group-item py-3 px-0 d-flex gap-3">
                        <span className="text-primary fw-bold min-w-100">{item.time}</span>
                        <div>
                          <h6 className="mb-1 fw-bold">{item.topic}</h6>
                          <p className="text-muted small mb-0">{item.speaker}</p>
                        </div>
                      </div>
                    ))}
                  </div>
                </div>
              )}
            </div>

            <div className="col-lg-4">
              <div className="card bg-light border-0 p-4 rounded-3 h-100 shadow-xs">
                <h3 className="h5 fw-bold mb-4">Event Details</h3>
                
                <ul className="list-unstyled mb-4">
                  <li className="mb-3 d-flex align-items-start gap-3">
                    <span className="p-2 rounded bg-primary-soft text-primary d-inline-flex">
                      <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-person-fill" viewBox="0 0 16 16">
                        <path d="M3 14s-1 0-1-1 1-4 6-4 6 3 6 4-1 1-1 1zm5-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6"/>
                      </svg>
                    </span>
                    <div>
                      <small className="text-muted d-block">Organized By</small>
                      <strong className="text-dark">{event.organizer || 'Event Committee'}</strong>
                    </div>
                  </li>

                  {event.capacity && (
                    <li className="mb-3 d-flex align-items-start gap-3">
                      <span className="p-2 rounded bg-primary-soft text-primary d-inline-flex">
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-people-fill" viewBox="0 0 16 16">
                          <path d="M7 14s-1 0-1-1 1-4 5-4 5 3 5 4-1 1-1 1zm4-6a3 3 0 1 0 0-6 3 3 0 0 0 0 6m-5.784 6A2.24 2.24 0 0 1 5 13c0-1.355.68-2.75 1.936-3.72A6.3 6.3 0 0 0 5 9c-4 0-5 3-5 4s1 1 1 1zM4.5 8a2.5 2.5 0 1 0 0-5 2.5 2.5 0 0 0 0 5"/>
                        </svg>
                      </span>
                      <div>
                        <small className="text-muted d-block">Capacity</small>
                        <strong className="text-dark">{event.capacity} seats</strong>
                      </div>
                    </li>
                  )}

                  {event.price !== undefined && (
                    <li className="mb-3 d-flex align-items-start gap-3">
                      <span className="p-2 rounded bg-primary-soft text-primary d-inline-flex">
                        <svg xmlns="http://www.w3.org/2000/svg" width="16" height="16" fill="currentColor" className="bi bi-tag-fill" viewBox="0 0 16 16">
                          <path d="M2 1a1 1 0 0 0-1 1v4.586a1 1 0 0 0 .293.707l7 7a1 1 0 0 0 1.414 0l4.586-4.586a1 1 0 0 0 0-1.414l-7-7A1 1 0 0 0 6.586 1zm4 3.5a1.5 1.5 0 1 1-3 0 1.5 1.5 0 0 1 3 0"/>
                        </svg>
                      </span>
                      <div>
                        <small className="text-muted d-block">Admission Price</small>
                        <strong className="text-dark">{event.price === 0 || event.price === 'Free' ? 'Free' : `$${event.price}`}</strong>
                      </div>
                    </li>
                  )}
                </ul>

                <button className="btn btn-primary w-100 rounded-pill py-2.5 fw-semibold btn-hover-scale shadow-sm">
                  Register for Event
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default EventDetail;
