import React, { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import {
  Box,
  Card,
  CardContent,
  Typography,
  Button,
  Menu,
  MenuItem,
  TextField,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
} from "@mui/material";
import { useSelector } from "react-redux";
import DeleteConfirmationDialog from "../DeleteConfirmationDialog/DeleteConfirmationDialog";
import "./PropertyList.css";

export default function LandingPage() {
  const navigate = useNavigate();
  const location = useLocation(); // Access location to get state

  const [cards, setCards] = useState([]);
  const [anchorEl, setAnchorEl] = React.useState(null);
  const [refreshKey, setRefreshKey] = useState(0);
  const [menuState, setMenuState] = useState({});
  const [successMessage, setSuccessMessage] = useState(null);
  const [selectedEstateId, setSelectedEstateId] = useState(null);

  const userId = useSelector((state) => state.id);
  const token = useSelector((state) => state.token);
  const open = Boolean(anchorEl);
  const [photoLink, setPhotoLink] = useState("");
  const [openDialog, setOpenDialog] = useState(false);

  // Set success message if available in location state
  useEffect(() => {
    if (location.state && location.state.successMessage) {
      setSuccessMessage(location.state.successMessage);

      // Clear the success message after 5 seconds
      const timer = setTimeout(() => setSuccessMessage(null), 5000);
      return () => clearTimeout(timer);
    }
  }, [location.state]);

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

  const handleOpenDialog = (estateId) => {
    setSelectedEstateId(estateId); // Set the selected estate ID for adding a photo
    setOpenDialog(true);
  };

  const handleCloseDialog = () => {
    setOpenDialog(false);
    setPhotoLink(""); // Clear the input field when the dialog is closed
  };

  const handleClick = (event, cardId) => {
    setMenuState({ [cardId]: event.currentTarget });
  };

  const handleClose = (cardId) => {
    setMenuState((prevState) => ({ ...prevState, [cardId]: null }));
  };
  const handleAddRoom = (estateId) => {
    setAnchorEl(null);
    console.log("estateid  " + estateId);
    navigate(`/add-room/${estateId}`);
  };

  const handleAddPhotos = async (estateId) => {
    if (!photoLink) {
      alert("Please provide a valid photo link.");
      return;
    }

    try {
      const response = await fetch(
        `http://localhost:5076/api/estate/${estateId}/add-photo`,
        {
          method: "POST",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({ Url: photoLink }), // Send the photo link in the request body
        }
      );

      if (!response.ok) {
        throw new Error("Failed to add photo.");
      }

      alert("Photo added successfully!");
      handleCloseDialog();
    } catch (error) {
      console.error("Error adding photo:", error);
      alert("Failed to add photo. Please try again.");
    }
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
      {successMessage && (
        <Box sx={{ textAlign: "center", marginBottom: 2 }}>
          <Typography variant="h6" color="success.main">
            {successMessage}
          </Typography>
        </Box>
      )}
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
                  id={`menu-button-${card.id}`}
                  aria-controls={`menu-${card.id}`}
                  aria-haspopup="true"
                  aria-expanded={Boolean(menuState[card.id])}
                  onClick={(event) => handleClick(event, card.id)}
                >
                  Edit
                </Button>
                <Menu
                  id={`menu-${card.id}`}
                  anchorEl={menuState[card.id] || null}
                  open={Boolean(menuState[card.id])}
                  onClose={() => handleClose(card.id)}
                  MenuListProps={{
                    "aria-labelledby": `menu-button-${card.id}`,
                  }}
                >
                  <MenuItem
                    onClick={handleOpenDialog}
                    // onClick={() => handleAddRoom(card.id)}
                  >
                    Add room
                  </MenuItem>
                  <MenuItem onClick={() => handleOpenDialog(card.id)}>
                    Add photos
                  </MenuItem>
                </Menu>
                {/* Dialog for adding photo */}
                <Dialog open={openDialog} onClose={handleCloseDialog}>
                  <DialogTitle>Add Photo</DialogTitle>
                  <DialogContent>
                    <TextField
                      autoFocus
                      margin="dense"
                      label="Photo URL"
                      type="url"
                      fullWidth
                      value={photoLink}
                      onChange={(e) => setPhotoLink(e.target.value)}
                      placeholder="Enter the photo URL"
                    />
                  </DialogContent>
                  <DialogActions>
                    <Button onClick={handleCloseDialog} color="secondary">
                      Cancel
                    </Button>
                    <Button onClick={handleAddPhotos} color="primary">
                      Add Photo
                    </Button>
                  </DialogActions>
                </Dialog>
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
