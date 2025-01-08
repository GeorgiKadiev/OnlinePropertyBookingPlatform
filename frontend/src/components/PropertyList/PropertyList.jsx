import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { Box, Card, CardContent, Typography, Button } from "@mui/material";
import { useSelector } from "react-redux"; // To access userId and token from Redux
import "./PropertyList.css";

export default function LandingPage() {
  const [cards, setCards] = useState([]); // State to hold the fetched data
  const [refreshKey, setRefreshKey] = useState(0);
  const navigate = useNavigate();
  const userId = useSelector((state) => state.id); 
  const token = useSelector((state) => state.token); 

  // Fetch estates on component mount
  useEffect(() => {
    const fetchEstates = async () => {
      try {
        const response = await fetch(
          `http://localhost:5076/api/estate/owner-estates/${userId}`,
          {
            method: "GET",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${token}`,
            },
          }
        );

        if (!response.ok) {
          throw new Error("Failed to fetch estates");
        }

        const data = await response.json();
        console.log(data);
        setCards(data);
      } catch (error) {
        console.error("Error fetching estates:", error);
      }
    };

    fetchEstates();
  }, [userId, token, refreshKey]); // Re-run effect if ...

  const refreshData = () => {
    setRefreshKey((prev) => prev + 1); // Increment to trigger re-fetch
  };

  const handleAdd = () => {
    navigate("/create-property");
  };

  const handleReservation = () => {
    navigate("/reservations");
  };

  const handleReviews = () => {
    navigate("/reviews");
  };

  const handleRemove = (estateId) => {
    // Add logic to remove an estate
    refreshData(); // Trigger re-fetch

    console.log("Remove estate with ID:", estateId);
  };

  return (
    <Box className="cards-container">
      {/* Button to add a new property */}
      <Card className="card">
        <CardContent className="card-content" sx={{ width: "60%", padding: 2 }}>
          <div className="card-buttons">
            <Button className="card-button" onClick={handleAdd}>
              Add new Property
            </Button>
          </div>
        </CardContent>
      </Card>

      {/* Render estate cards */}
      {cards.map((card) => (
        <Card className="card" key={card.id}>
          {/* Image on the left */}
          <Box
            sx={{
              width: "15%",
              display: "flex",
              justifyContent: "center",
              alignItems: "center",
            }}
          >
            <img
              src={card.imageUrl || "https://via.placeholder.com/150"} // Use card.imageUrl if available
              alt={card.name}
              style={{ width: "100%", height: "auto", objectFit: "cover" }}
            />
          </Box>

          {/* Content on the right */}
          <CardContent
            className="card-content"
            sx={{ width: "60%", padding: 2 }}
          >
            <Typography variant="h5" className="card-title">
              {card.title}
            </Typography>
            <Typography variant="body2" className="card-description">
              {card.description || "No description available"} 
            </Typography>
            <div className="card-buttons">
              <Button className="card-button" onClick={handleReservation}>
                Reservations
              </Button>
              <Button className="card-button" onClick={handleReviews}>
                Reviews
              </Button>
              <Button
                className="card-button"
                onClick={() => handleRemove(card.id)} // Pass estate ID to remove
              >
                Remove estate
              </Button>
            </div>
          </CardContent>
        </Card>
      ))}
    </Box>
  );
}
