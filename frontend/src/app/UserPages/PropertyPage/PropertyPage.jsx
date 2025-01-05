import React from "react";
import { useParams } from "react-router-dom";
import { Box, Typography, Button, Grid, Paper } from "@mui/material";

export default function PropertyPage() {
  const { id } = useParams();

  // Simulated property data for demonstration
  const property = {
    id,
    title: `Property ${id}`,
    description: `This is a beautiful property with modern amenities, located in a prime area. It's perfect for a relaxing getaway or a business trip.`,
    price: "$100/night",
    images: [
      "https://via.placeholder.com/300x200",
      "https://via.placeholder.com/300x200",
      "https://via.placeholder.com/300x200",
    ],
  };

  return (
    <Box sx={{ p: 3 }}>
      {/* Title */}
      <Typography variant="h4" sx={{ mb: 2 }}>
        {property.title}
      </Typography>

      {/* Image Section */}
      <Grid container spacing={2} sx={{ mb: 3 }}>
        {property.images.map((image, index) => (
          <Grid item xs={12} sm={6} md={4} key={index}>
            <Paper elevation={3} sx={{ overflow: "hidden" }}>
              <img
                src={image}
                alt={`Property image ${index + 1}`}
                style={{ width: "100%", height: "200px", objectFit: "cover" }}
              />
            </Paper>
          </Grid>
        ))}
      </Grid>

      {/* Description and Booking */}
      <Box sx={{ display: "flex", justifyContent: "space-between", alignItems: "center", mb: 2 }}>
        <Box sx={{ flex: 1, pr: 2 }}>
          <Typography variant="body1" color="text.secondary" sx={{ mb: 1 }}>
            {property.description}
          </Typography>
          <Typography variant="h6" color="primary">
            {property.price}
          </Typography>
        </Box>
        <Button
          variant="contained"
          color="primary"
          size="large"
          sx={{ alignSelf: "flex-start" }}
          onClick={() => alert(`Booking Property ${id}`)}
        >
          Book
        </Button>
      </Box>
    </Box>
  );
}
