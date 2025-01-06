import React, { useState } from "react";
import { useNavigate } from "react-router-dom";
import { Box, Card, CardContent, Typography, Button } from "@mui/material";
import "./PropertyList.css";

const cards = [
  {
    title: "Card 1",
    description: "This is the description for card 1.",
    image: "https://via.placeholder.com/150",
  },
  {
    title: "Card 2",
    description: "This is the description for card 2.",
    image: "https://via.placeholder.com/150",
  },
  {
    title: "Card 3",
    description: "This is the description for card 3.",
    image: "https://via.placeholder.com/150",
  },
];

export default function LandingPage() {
  const navigate = useNavigate();

  const handleAdd = () => {
    navigate("/create-property");
  };

  return (
    <Box className="cards-container">
      <Card className="card">
        <CardContent className="card-content" sx={{ width: "60%", padding: 2 }}>
          <div className="card-buttons">
            <Button className="card-button" onClick={handleAdd}>
              Add new Propery
            </Button>
          </div>
        </CardContent>
      </Card>

      {cards.map((card, index) => (
        <Card className="card" key={index}>
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
              src={card.image}
              alt={card.title}
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
              {card.description}
            </Typography>
            <div className="card-buttons">
              <Button className="card-button">Button 1</Button>
              <Button className="card-button">Button 2</Button>
            </div>
          </CardContent>
        </Card>
      ))}
    </Box>
  );
}
