import React, { useState, useEffect } from "react";
import { useSelector } from "react-redux";
import { useParams } from "react-router-dom";
import {
  Box,
  Typography,
  Button,
  Grid,
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
  }, [estateId]);

  const handleBooking = async (roomId) => {
    try {
      const response = await fetch(
        `http://localhost:5076/api/reservation/create/${estateId}/${roomId}`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (!response.ok) {
        throw new Error("Failed to book the room");
      }

      alert("Room booked successfully!");
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
        <Grid container spacing={2} className="property-images">
          {property.images &&
            property.images.map((image, index) => (
              <Grid item xs={12} sm={6} md={4} key={index}>
                <Paper elevation={3} className="image-paper">
                  <img
                    src={image}
                    alt={`Property image ${index + 1}`}
                    className="property-image"
                  />
                </Paper>
              </Grid>
            ))}
        </Grid>

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
          <Grid container spacing={2}>
            {rooms.map((room) => (
              <Grid item xs={12} sm={6} md={4} key={room.id}>
                <Paper elevation={3} className="room-card">
                  <Box className="room-info">
                    <Typography variant="h6">Room {room.name}</Typography>
                    <Typography>Max Guests: {room.maxGuests}</Typography>
                    <Typography>
                      Price: ${room.pricePerNight} per night
                    </Typography>
                  </Box>
                  <Button
                    variant="contained"
                    color="primary"
                    onClick={() => handleBooking(room.id)} // Pass roomId to booking handler
                  >
                    Book
                  </Button>
                </Paper>
              </Grid>
            ))}
          </Grid>
        </Box>

        {/* Reviews Section */}
        <Reviews estateId={estateId} />
      </Box>
    </div>
  );
}
