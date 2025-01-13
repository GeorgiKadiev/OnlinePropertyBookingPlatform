import React, { useState, useEffect } from "react";
import { useSelector } from "react-redux";
import { useParams } from "react-router-dom";
import { DatePicker } from "@mui/x-date-pickers/DatePicker";
import { LocalizationProvider } from "@mui/x-date-pickers/LocalizationProvider";
import { AdapterDayjs } from "@mui/x-date-pickers/AdapterDayjs";
import dayjs from "dayjs";
import {
  Box,
  Typography,
  Button,
  Grid2,
  Paper,
  CircularProgress,
} from "@mui/material";
import Reviews from "../../../components/EstateReviews/EstateReviews";
import NavBar from "../../../components/NavBar/NavBar";
import "./PropertyPage.css";

export default function PropertyPage() {
  const { id: estateId } = useParams();
  const token = useSelector((state) => state.token);
  const [property, setProperty] = useState(null);
  const [rooms, setRooms] = useState([]);
  const [bookedDates, setBookedDates] = useState({}); // Track booked dates
  const [selectedDates, setSelectedDates] = useState({});
  const [activeRoom, setActiveRoom] = useState(null); // Room for which date pickers are shown
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  // Fetch estate details and rooms
  useEffect(() => {
    const fetchProperty = async () => {
      try {
        const propertyResponse = await fetch(
          `http://localhost:5076/api/estate/${estateId}`
        );
        if (!propertyResponse.ok) {
          throw new Error("Property not found");
        }
        const propertyData = await propertyResponse.json();
        setProperty(propertyData);

        const roomsResponse = await fetch(
          `http://localhost:5076/api/estate/${estateId}/rooms`,
          {
            method: "GET",
            headers: {
              Authorization: `Bearer ${token}`,
            },
          }
        );
        if (!roomsResponse.ok) {
          throw new Error("Rooms not found");
        }
        const roomsData = await roomsResponse.json();
        setRooms(roomsData);

        setLoading(false);
      } catch (err) {
        setError(err.message);
        setLoading(false);
      }
    };

    fetchProperty();
  }, [estateId, token]);

  // Fetch booked dates for a room
  const fetchBookedDates = async (roomId) => {
    try {
      const response = await fetch(
        `http://localhost:5076/api/room/details/${roomId}/dates`
      );
      if (!response.ok) {
        throw new Error("Failed to fetch booked dates");
      }
      const data = await response.json();

      setBookedDates((prev) => ({ ...prev, [roomId]: data }));
    } catch (err) {
      console.error("Error fetching booked dates:", err);
    }
  };

  const handleBooking = (roomId) => {
    setActiveRoom(roomId); // Show date pickers for this room
    if (!bookedDates[roomId]) {
      fetchBookedDates(roomId); // Fetch booked dates if not already fetched
    }
  };

  const confirmBooking = async (roomId) => {
    const { startDate, endDate } = selectedDates[roomId] || {};

    if (!startDate || !endDate) {
      alert("Please select start and end dates for booking.");
      return;
    }
    const payload = JSON.stringify({
      CheckInDate: dayjs(startDate).format("YYYY-MM-DD"),
      CheckOutDate: dayjs(endDate).format("YYYY-MM-DD"),
    });
    console.log(payload);

    try {
      const response = await fetch(
        `http://localhost:5076/api/reservation/create/${estateId}/${roomId}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: payload,
        }
      );

      if (!response.ok) {
        throw new Error("Failed to book the room");
      }

      alert("Room booked successfully!");
      setActiveRoom(null); // Hide date pickers
    } catch (err) {
      console.error("Error booking room:", err);
      alert("Failed to book the room. Please try again.");
    }
  };

  if (loading) {
    return (
      <Box className="loading-container">
        <CircularProgress />
      </Box>
    );
  }

  if (error) {
    return (
      <Box className="error-container">
        <Typography variant="h6" color="error">
          Error: {error}
        </Typography>
      </Box>
    );
  }

  return (
    <div>
      <NavBar />
      <Box className="property-page">
        {/* Title */}
        <Typography variant="h4" className="property-title">
          {property.title}
        </Typography>

        {/* Image Section */}
        <Grid2 container spacing={2} className="property-images">
          {property.photos &&
            property.photos.map((image, index) => (
              <Grid2 item xs={12} sm={6} md={4} key={index}>
                <Paper elevation={3} className="image-paper">
                  <img
                    src={image}
                    alt={`Property image ${index + 1}`}
                    className="property-image"
                  />
                </Paper>
              </Grid2>
            ))}
        </Grid2>

        {/* Description */}
        <Box className="property-details">
          <Typography variant="body1" className="property-description">
            {property.description}
          </Typography>
          <Typography variant="h6" className="property-price">
            Price per night: ${property.price}
          </Typography>
        </Box>

        {/* Rooms Section */}
        <Box className="rooms-section">
          <Typography variant="h5" className="rooms-title">
            Available Rooms
          </Typography>
          <Grid2 container spacing={2}>
            {rooms.map((room) => (
              <Grid2 item xs={12} sm={6} md={4} key={room.id}>
                <Paper elevation={3} className="room-card">
                  <Box className="room-info">
                    <Typography variant="h6">Room {room.name}</Typography>
                    <Typography>Max Guests: {room.maxGuests}</Typography>
                    <Typography>
                      Price: ${room.pricePerNight} per night
                    </Typography>
                    {/* Image Section */}
                    <Grid2 container spacing={2} className="property-images">
                      {room.photos &&
                        room.photos.map((image, index) => (
                          <Grid2 item xs={12} sm={6} md={4} key={index}>
                            <Paper elevation={3} className="image-paper">
                              <img
                                src={image}
                                alt={`Property image ${index + 1}`}
                                className="property-image"
                              />
                            </Paper>
                          </Grid2>
                        ))}
                    </Grid2>
                  </Box>

                  {activeRoom === room.id ? (
                    <LocalizationProvider dateAdapter={AdapterDayjs}>
                      <Box className="date-pickers">
                        <DatePicker
                          label="Start Date"
                          value={selectedDates[room.id]?.startDate || null}
                          onChange={(date) =>
                            setSelectedDates((prev) => ({
                              ...prev,
                              [room.id]: { ...prev[room.id], startDate: date },
                            }))
                          }
                          shouldDisableDate={(date) =>
                            bookedDates[room.id]?.includes(
                              date.format("YYYY-MM-DD")
                            )
                          }
                        />
                        <DatePicker
                          label="End Date"
                          value={selectedDates[room.id]?.endDate || null}
                          onChange={(date) =>
                            setSelectedDates((prev) => ({
                              ...prev,
                              [room.id]: { ...prev[room.id], endDate: date },
                            }))
                          }
                          shouldDisableDate={(date) =>
                            bookedDates[room.id]?.includes(
                              date.format("YYYY-MM-DD")
                            )
                          }
                        />
                      </Box>
                      <Button
                        variant="contained"
                        color="secondary"
                        onClick={() => confirmBooking(room.id)}
                      >
                        Confirm
                      </Button>
                    </LocalizationProvider>
                  ) : (
                    <Button
                      variant="contained"
                      color="primary"
                      onClick={() => handleBooking(room.id)}
                    >
                      Book
                    </Button>
                  )}
                </Paper>
              </Grid2>
            ))}
          </Grid2>
        </Box>

        {/* Reviews Section */}
        <Reviews estateId={estateId} />
      </Box>
    </div>
  );
}
