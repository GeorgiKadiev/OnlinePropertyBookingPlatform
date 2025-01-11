import React, { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import {
  Box,
  Card,
  CardContent,
  Typography,
  Button,
  Menu,
  MenuItem,
} from "@mui/material";
import { useSelector } from "react-redux";
import DeleteConfirmationDialog from "../DeleteConfirmationDialog/DeleteConfirmationDialog";
import "./PropertyList.css";

export default function LandingPage() {
  const [cards, setCards] = useState([]);
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [refreshKey, setRefreshKey] = useState(0);
  const navigate = useNavigate();
  const userId = useSelector((state) => state.id);
  const token = useSelector((state) => state.token);
  const open = Boolean(anchorEl);

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
        setCards(data);
      } catch (error) {
        console.error("Error fetching estates:", error);
      }
    };

    fetchEstates();
  }, [userId, token, refreshKey]);

  const refreshData = () => {
    setRefreshKey((prev) => prev + 1);
  };

  const handleClick = (event) => {
    setAnchorEl(event.currentTarget);
  };
  const handleClose = () => {
    setAnchorEl(null);
  };
  const handleAddRoom = (estateId) => {
    setAnchorEl(null);
    console.log("estateid  " + estateId);
    navigate(`/add-room/${estateId}`);
  };
  const handleAddPhotos = (estateId) => {
    setAnchorEl(null);
  };
  const handleRemove = async (estateId) => {
    try {
      const response = await fetch(
        `http://localhost:5076/api/estate/${estateId}`,
        {
          method: "DELETE",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (!response.ok) {
        throw new Error(`Failed to delete estate with ID: ${estateId}`);
      }

      console.log(`Estate with ID: ${estateId} successfully deleted.`);
      refreshData();
    } catch (error) {
      console.error("Error deleting estate:", error);
    }
  };

  return (
    <Box className="cards-container">
      <Card className="card">
        <CardContent className="card-content" sx={{ width: "60%", padding: 2 }}>
          <div className="card-buttons">
            <Button
              className="card-button"
              onClick={() => navigate("/create-property")}
            >
              Add new Property
            </Button>
          </div>
        </CardContent>
      </Card>
      {cards.length === 0 ? (
        <Box sx={{ textAlign: "center", marginTop: 4 }}>
          <Typography variant="h6" color="textSecondary">
            You have no estates. Create one to get started!
          </Typography>
        </Box>
      ) : (
        cards.map((card) => (
          <Card className="card" key={card.id}>
            <Box
              sx={{
                width: "15%",
                display: "flex",
                justifyContent: "center",
                alignItems: "center",
              }}
            >
              <img
                src={card.imageUrl || "https://via.placeholder.com/150"}
                alt={card.name}
                style={{ width: "100%", height: "auto", objectFit: "cover" }}
              />
            </Box>

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
                <Button
                  className="card-button"
                  aria-controls={open ? "basic-menu" : undefined}
                  aria-haspopup="true"
                  aria-expanded={open ? "true" : undefined}
                  onClick={handleClick}
                >
                  Edit
                </Button>
                <Menu
                  id="basic-menu"
                  anchorEl={anchorEl}
                  open={open}
                  onClose={handleClose}
                  MenuListProps={{
                    "aria-labelledby": "basic-button",
                  }}
                >
                  <MenuItem onClick={() => handleAddRoom(card.id)}>
                    Add room
                  </MenuItem>
                  <MenuItem onClick={() => handleAddPhotos(card.id)}>
                    Add photos
                  </MenuItem>
                </Menu>
                <Button
                  className="card-button"
                  onClick={() => navigate("/reservations")}
                >
                  Reservations
                </Button>
                <Button
                  className="card-button"
                  onClick={() => navigate(`/reviews/${card.id}`)}
                >
                  Reviews
                </Button>
                <DeleteConfirmationDialog
                  estateId={card.id}
                  onConfirm={handleRemove}
                />
              </div>
            </CardContent>
          </Card>
        ))
      )}
    </Box>
  );
}
